<template>
  <div class="card">
    <div v-if="loading" class="d-flex flex-column align-items-center" style="padding: 100px 0">
      <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
      <h3 class="m-0">Đang tải dữ liệu...</h3>
    </div>
    <div v-else>
      <div class="card-header">
        <div class="row px-2 justify-content-between">
          <div class="col-12 col-md-12 col-lg-2 mb-2 p-0">
            <input v-model="searchQuery" class="form-control" placeholder="Tìm kiếm" />
          </div>
          <div class="col-12 col-md-12 col-lg-4 p-0">
            <div class="d-flex flex-row-reverse">
              <router-link :to="{ name: 'admin_room_new' }" class="btn btn-sm btn-primary ms-3"><i class="fa-solid fa-plus me-1"></i>Thêm</router-link>
              <select v-model="pageSize" class="form-select ms-3 w-25">
                <option value="5">5</option>
                <option value="10">10</option>
                <option value="15">15</option>
                <option value="20">20</option>
              </select>
              <select v-model="filter" @change="filterItems" class="form-select">
                <option v-for="option in filterOption" :key="option" :value="option">{{ option }}</option>
              </select>
            </div>
          </div>
        </div>
      </div>
      <div v-if="items.length == 0" class="d-flex flex-column align-items-center" style="padding: 100px 0">
        <i class="fa-solid fa-face-thinking text-warning mb-5" style="font-size: 150px"></i>
        <h3 class="m-0">Không tìm thấy dữ liệu nào !</h3>
      </div>
      <div v-else class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>Phòng</th>
              <th>Tên phòng</th>
              <th>Tên chi nhánh</th>
              <th style="width: 110px" class="text-center">Số người tối đa</th>
              <th class="text-center">Giá tiền</th>
              <th class="text-center">Kích thước</th>
              <th style="width: 130px" class="text-center">Trạng thái</th>
              <th style="width: 150px">Ngày tạo</th>
              <th style="width: 150px">Ngày cập nhật</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in items" :key="item.id">
              <td>
                <span class="fw-bold">{{ item.room_number }}</span>
              </td>
              <td>
                <span class="fw-bold">{{ item.room_name }}</span>
              </td>
              <td>
                <span class="fw-bold">{{ item.branch.name }}</span>
              </td>
              <td class="text-center">{{ item.room_max_adults }} người</td>
              <td class="text-center">{{ formatCurrencyVND(item.cost) }}</td>
              <td class="text-center">{{ item.room_width }}m<sup>2</sup></td>
              <td class="text-center">
                <span class="badge" :class="[!item.is_locked ? 'bg-label-success' : 'bg-label-danger']">{{ !item.is_locked ? "Đang mở" : "Đã đóng" }}</span>
              </td>
              <td v-html="item.updated_at"></td>
              <td v-html="item.created_at"></td>
              <td>
                <div class="d-flex justify-content-sm-end align-items-sm-center">
                  <button class="btn btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                    <i class="bx bx-dots-vertical-rounded bx-md"></i>
                  </button>
                  <div class="dropdown-menu dropdown-menu-end m-0">
                    <button type="button" class="dropdown-item" @click="toggle_status(item)">{{ item.is_locked ? "Kích hoạt" : "Vô hiệu hoá" }}</button>
                    <router-link :to="{ name: 'admin_room_update', query: { id: item.id, name: `${item.room_number} - ${item.branch.name}` } }" class="dropdown-item">Chỉnh sửa</router-link>
                    <button type="button" data-bs-toggle="modal" data-bs-target="#modal-delete" @click="room_delete = item" class="dropdown-item delete-record">Xoá</button>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        <div class="d-flex justify-content-lg-end justify-content-sm-end align-items-center">
          <ul class="pagination m-5">
            <ul class="pagination">
              <li class="page-item previous" :class="{ disabled: currentPage == 1 }" @click="changePage(currentPage - 1)">
                <button class="page-link" type="button" tabindex="-1">
                  <i class="bx bx-chevron-left bx-18px"></i>
                </button>
              </li>
              <li v-for="page in getPageRange" :key="page" class="page-item" :class="{ active: page === currentPage }" @click="changePage(page)">
                <button class="page-link" role="link" type="button">{{ page }}</button>
              </li>
              <li class="page-item" :class="{ disabled: currentPage == totalPages }" @click="changePage(currentPage + 1)">
                <button class="page-link" role="link" type="button">
                  <i class="bx bx-chevron-right bx-18px"></i>
                </button>
              </li>
            </ul>
          </ul>
        </div>
      </div>
    </div>
  </div>
  <div class="modal fade" id="modal-delete" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="modalCenterTitle">Xoá phòng</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal" @click="close_modal_delete" aria-label="Close"></button>
        </div>
        <div class="modal-body">Bạn có chắc chắn muốn xóa phòng này không?</div>
        <div class="modal-footer">
          <button type="button" class="btn btn-label-secondary" @click="close_modal_delete" data-bs-dismiss="modal">Huỷ</button>
          <button type="button" class="btn btn-primary" @click="confirm_delete" data-bs-dismiss="modal">Xoá</button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted, watch, computed } from "vue";
import axios from "@/utils/axios";
import formatDateTime from "@/utils/formatDateTime.js";
import breadcrumb from "@/utils/breadcrumb";
import { toast } from "vue-sonner";
import formatCurrencyVND from "@/utils/formatCurrencyVND";

breadcrumb([{ path: "admin_room_list", name: "Danh sách phòng ngủ" }]);

const items = ref([]);
const loading = ref(false);

const searchQuery = ref("");
const filter = ref("Tất cả");
const filterOption = ref(["Tất cả", "Cập nhật mới nhất", "Giá tăng dần", "Giá giảm dần", "Diện tích tăng dần", "Diện tích giảm dần"]);
const totalItems = ref(1);
const totalPages = ref(1);
const currentPage = ref(1);
const pageSize = ref(5);
const room_delete = ref(null);

const toggle_status = async (item) => {
  try {
    const response = await axios.put(`/room/toggle-room/${item.id}`, null, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });
    if (response.data.success) {
      toast.success("Cập nhật trạng thái thành công !");
      items.value.find((i) => i.id === item.id).is_locked = !item.is_locked;
    } else toast.error(response.data.message);
  } finally {
  }
};

const fetchData = async () => {
  try {
    loading.value = true;
    const response = await axios.get("/room/get-pagination", {
      params: {
        page: currentPage.value,
        pageSize: pageSize.value,
        search: searchQuery.value,
        filter: filter.value,
      },
    });
    if (response.data.success) {
      const data = response.data.data;

      items.value = data.items.map((item) => ({
        ...item,
        updated_at: formatDateTime(item.updated_at),
        created_at: formatDateTime(item.created_at),
      }));

      totalPages.value = data.totalPages;
      totalItems.value = data.totalItems;
    } else toast.error(response.data.message);
  } finally {
    loading.value = false;
  }
};

watch([filter, pageSize], () => {
  currentPage.value = 1;
  fetchData();
});

const debounce = (func, delay) => {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      func.apply(this, args);
    }, delay);
  };
};

const searchItems = debounce(() => {
  currentPage.value = 1;
  fetchData();
}, 300);

watch(searchQuery, searchItems);

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return;
  currentPage.value = page;
  fetchData();
};

const getPageRange = computed(() => {
  const range = [];
  const rangeSize = 4;
  let rangeStart = Math.max(1, currentPage.value - Math.floor(rangeSize / 2));
  const rangeEnd = Math.min(totalPages.value, rangeStart + rangeSize - 1);
  if (rangeEnd - rangeStart + 1 < rangeSize) {
    rangeStart = Math.max(1, rangeEnd - rangeSize + 1);
  }
  for (let i = rangeStart; i <= rangeEnd; i++) {
    range.push(i);
  }
  return range;
});

function close_modal_delete() {
  if (room_delete.value != null) {
    room_delete.value = null;
  }
}

const confirm_delete = async () => {
  if (room_delete.value != null) {
    try {
      const response = await axios.delete(`/room/${room_delete.value.id}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });
      if (response.data.success) {
        toast.success("Xoá phòng thành công !");
        fetchData();
      } else toast.error(response.data.message);
    } finally {
      room_delete.value = null;
    }
  } else toast.warning("Đã có lỗi xảy ra khi xử lý dữ liệu. Vui lòng tải lại trang và thử lại !");
};

onMounted(async () => {
  window.addEventListener("keydown", close_modal_delete);
  fetchData();
});
</script>
