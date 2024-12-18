<template>
  <div class="row m-0 d-flex align-items-stretch mb-5">
    <div class="col-lg-8 col-8 col-sm-8 p-0 pe-2">
      <div class="card card-flush h-100">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h4 class="mb-0">Điều chỉnh tiện nghi</h4>
          <div class="d-flex align-items-center gap-2 gap-lg-3">
            <button :disabled="editing === 0" @click="edit_cancel" class="btn btn-sm btn-flex btn-secondary fw-bold">Huỷ</button>
            <button v-if="edit_loading" disabled class="btn btn-sm fw-bold btn-primary">
              <div class="spinner-border border-4 w-20px h-20px mt-1"></div>
            </button>
            <button v-else @click="edit_save" class="btn btn-sm fw-bold btn-primary">Lưu</button>
          </div>
        </div>
        <div class="card-body pt-0">
          <div class="form-group mb-5">
            <label class="fw-bold fs-6 form-label">Icon</label>
            <input type="text" v-model="icon_preview" class="form-control form-control-solid mb-3" placeholder="fa-duotone fa-solid fa-wifi" />
            <p class="mb-1 text-muted">Hướng dẫn: tìm kiếm và ấn vào Icon cần thêm vào và làm như sau:</p>
            <p class="mb-1 text-muted">- Bạn sẽ thấy đoạn code sau hiển thị lên <b>&lt;i class="fa-duotone fa-solid fa-wifi"&gt;&lt;/i&gt;</b></p>
            <p class="mb-1 text-muted">- Hãy lấy <b>fa-duotone fa-solid fa-wifis</b> dán vào đây.</p>
            <p class="mb-1 text-muted">Tham khảo icon <a href="https://fontawesome.com/icons/" target="_blank">tại đây.</a></p>
          </div>
          <div class="form-group mb-5">
            <label class="fw-bold fs-6 form-label">Tên</label>
            <input type="text" v-model="facility_name" class="form-control form-control-solid mb-3" placeholder="Wifi, hồ bơi, BBQ,..." />
          </div>
        </div>
      </div>
    </div>
    <div class="col-lg-4 col-4 col-sm-4 p-0 ps-2">
      <div class="card card-flush h-100">
        <div class="card-header align-items-center">
          <div class="card-title">
            <h4 class="mb-0">Xem trước</h4>
          </div>
        </div>
        <div class="card-body pt-0">
          <div class="d-flex flex-column align-items-center">
            <i :class="icon_preview" class="mb-5" style="font-size: 13vw"></i>
            <span class="text-center fs-5">{{ icon_preview }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div class="card">
    <div v-if="loading" class="d-flex flex-column align-items-center" style="padding: 100px 0">
      <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
      <h3 class="m-0">Đang tải dữ liệu...</h3>
    </div>
    <div v-else>
      <div class="card-header">
        <div class="row px-2 justify-content-between">
          <div class="col-12 col-lg-2 mb-2 p-0">
            <input v-model="searchQuery" class="form-control" placeholder="Tìm kiếm" />
          </div>
          <div class="col-12 col-lg-3 p-0 ps-3">
            <div class="d-flex flex-row-reverse">
              <select v-model="filter" @change="filterItems" class="form-select">
                <option v-for="option in filterOption" :key="option" :value="option">{{ option }}</option>
              </select>
              <select v-model="pageSize" class="form-select me-3">
                <option value="5">5</option>
                <option value="10">10</option>
                <option value="15">15</option>
                <option value="20">20</option>
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
              <th>#</th>
              <th>Icon</th>
              <th class="text-end">Tên</th>
              <th class="text-end">Ngày cập nhật</th>
              <th class="text-end">Ngày khởi tạo</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in items" :key="item.id">
              <td class="text-start">
                <span class="fw-bold">{{ item.id }}.</span>
              </td>
              <td class="text-start">
                <i :class="[item.icon, 'pe-3']" style="font-size: 30px"></i>
                <span class="fw-bold">{{ item.icon }}</span>
              </td>
              <td class="text-end">
                <span class="text-gray-800 fw-bold"> {{ item.name }}</span>
              </td>
              <td class="text-end pe-0" v-html="item.updated_at"></td>
              <td class="text-end pe-0" v-html="item.created_at"></td>
              <td>
                <div class="d-flex justify-content-sm-end align-items-sm-center">
                  <button class="btn btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                    <i class="bx bx-dots-vertical-rounded bx-md"></i>
                  </button>
                  <div class="dropdown-menu dropdown-menu-end m-0">
                    <button type="button" @click="edit_item(item)" class="dropdown-item">Edit</button>
                    <button type="button" @click="edit_del(item)" class="dropdown-item delete-record">Delete</button>
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
</template>

<script setup>
import { ref, onMounted, computed, watch } from "vue";
import { toast } from "vue-sonner";
import axios from "@/utils/axios.js";
import formatDateTime from "@/utils/formatDateTime.js";
import breadcrumb from "@/utils/breadcrumb";

breadcrumb([{ path: "admin_room_facility", name: "Tiện nghi" }]);

document.title = "Tiện nghi";

const icon_preview = ref("fa-duotone fa-solid fa-wifi");
const facility_name = ref("");
const editing = ref(0);
const edit_loading = ref(false);

const edit_save = async () => {
  console.log(icon_preview.value, facility_name.value);
  if (!icon_preview.value || !facility_name.value) toast.error("Vui lòng nhập đầy đủ thông tin !");
  else {
    edit_loading.value = true;
    if (editing.value === 0) {
      try {
        const response = await axios.post(
          "/facilities",
          {
            icon: icon_preview.value,
            name: facility_name.value,
          },
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
            },
          }
        );

        const data = response.data;
        if (data.success) {
          toast.success(data.message);
          fetchData();
          edit_cancel();
        } else toast.error(data.message);
      } finally {
        edit_loading.value = false;
      }
    } else {
      try {
        const response = await axios.put(
          `/facilities/${editing.value}`,
          {
            icon: icon_preview.value,
            name: facility_name.value,
          },
          {
            headers: {
              Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
            },
          }
        );

        const data = response.data;
        if (data.success) {
          toast.success(data.message);
          fetchData();
          edit_cancel();
        } else toast.error(data.message);
      } finally {
        edit_loading.value = false;
      }
    }
  }
};

const edit_del = async (item) => {
  if (confirm("Bạn có chắc chắn muốn xóa tiện nghi này không ?")) {
    try {
      const response = await axios.delete(`/facilities/${item.id}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });

      const data = response.data;
      if (data.success) {
        toast.success(data.message);
        fetchData();
      } else toast.error(data.message);
    } finally {
      edit_cancel();
    }
  }
};

const edit_cancel = () => {
  editing.value = 0;
  icon_preview.value = "fa-duotone fa-solid fa-wifi";
  facility_name.value = "";
};

const edit_item = (item) => {
  editing.value = item.id;
  icon_preview.value = item.icon;
  facility_name.value = item.name;
  toast.info("Bạn đã chọn tiện nghi này, hãy chỉnh sửa nó ở trên !");
};

// Pagination
const items = ref([]);
const loading = ref(false);

const searchQuery = ref("");
const filter = ref("Tất cả");
const filterOption = ref(["Tất cả", "Cập nhật mới nhất", "Cập nhật cũ nhất"]);
const totalItems = ref(1);
const totalPages = ref(1);
const currentPage = ref(1);
const pageSize = ref(5);

const fetchData = async () => {
  loading.value = true;
  try {
    const response = await axios.get("/facilities/get-pagination", {
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
    } else {
      toast.error(response.data.message);
    }
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

onMounted(() => {
  fetchData();
});
</script>
