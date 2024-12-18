<template>
  <div class="card">
    <div class="card-header">
      <div class="d-flex justify-content-between">
        <h4 class="m-0">Tài khoản khách hàng</h4>
        <span>
          <router-link :to="{ name: 'admin_customer_list' }" type="button" class="btn btn-sm btn-secondary me-2">Trở về</router-link>
        </span>
      </div>
    </div>
    <div v-if="loading_data" class="d-flex flex-column align-items-center" style="padding: 100px 0">
      <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
      <h3 class="m-0">Đang tải dữ liệu...</h3>
    </div>
    <div v-else class="card-body">
      <div class="row">
        <div class="col-6">
          <div class="row g-6">
            <div class="col-12">
              <label class="form-label" for="full_name">Họ tên</label>
              <input disabled :value="user.full_name" type="text" id="full_name" class="form-control" placeholder="Họ và tên" />
            </div>
            <div class="col-12">
              <label class="form-label" for="phone_number">Số điện thoại</label>
              <input disabled :value="user.phone_number" type="text" id="phone_number" class="form-control" placeholder="Số điện thoại" />
            </div>
            <div class="col-12">
              <label class="form-label" for="email">Email</label>
              <input disabled :value="user.email" type="text" id="email" class="form-control" placeholder="Email" />
            </div>
          </div>
        </div>
        <div class="col-6">
          <div class="row g-6">
            <div class="col-12">
              <label class="form-label" for="name">Ngày tạo tài khoản</label>
              <input disabled type="text" :value="user.created_at.replace('T', ' ')" id="created_at" class="form-control" />
            </div>
            <div class="col-12">
              <label class="form-label" for="name">Ngày cập nhật cuối cùng</label>
              <input disabled type="text" :value="user.updated_at.replace('T', ' ')" id="updated_at" class="form-control" />
            </div>
            <div class="col-12">
              <label class="form-label" for="name">Chức vụ</label>
              <input disabled type="text" :value="user.role == 0 ? 'Khách hàng' : user.role == 1 ? 'Nhân viên' : 'Quản lý'" id="updated_at" class="form-control" />
            </div>
          </div>
        </div>
      </div>
      <div class="row mt-5">
        <div class="col-2">
          <label class="form-label" for="reward_points">Điểm đã tích</label>
          <input disabled :value="user.reward_points + ' điểm'" type="text" id="reward_points" class="form-control" />
        </div>
        <div class="col-5">
          <div class="row">
            <div class="col-9">
              <label class="form-label" for="is_locked">Trạng thái tài khoản</label>
              <input disabled :value="user.is_locked ? 'Bị cấm' : 'Hoạt động'" type="text" id="is_locked" class="form-control" :class="user.is_locked ? 'border-danger text-danger' : 'border-success text-success'" />
            </div>
            <div class="col-3">
              <label class="form-label" for="is_verified">{{ user.is_locked ? "Mở chặn ngay" : "Vô hiệu hoá ngay" }}</label>
              <div class="d-flex justify-content-start w-100">
                <button @click="lock_user_now" class="btn btn-lg w-100" :class="user.is_locked ? 'btn-primary' : 'btn-danger'">
                  <div v-if="loading_active" class="spinner-border" role="status"></div>
                  <i v-else :class="user.is_locked ? 'fa-solid fa-check' : 'fa-solid fa-ban'"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
        <div v-if="!user.is_verified" class="col-5">
          <div class="row">
            <div class="col-9">
              <label class="form-label" for="is_verified">Kích hoạt</label>
              <input disabled :value="user.is_verified ? 'Đã kích hoạt' : 'Chưa kích hoạt'" type="text" id="is_verified" class="form-control" :class="user.is_verified ? 'border-success text-success' : 'border-warning text-warning'" />
            </div>
            <div class="col-3">
              <label class="form-label" for="is_verified">Kích hoạt ngay</label>
              <div class="d-flex justify-content-center w-100">
                <button @click="active_now" class="btn btn-lg btn-primary w-100">
                  <div v-if="loading_active" class="spinner-border" role="status"></div>
                  <i v-else class="fa-solid fa-check"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="col-5">
          <label class="form-label" for="is_verified">Kích hoạt</label>
          <input disabled :value="user.is_verified ? 'Đã kích hoạt' : 'Chưa kích hoạt'" type="text" id="is_verified" class="form-control" :class="user.is_verified ? 'border-success text-success' : 'border-warning text-warning'" />
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import axios from "@/utils/axios";
import breadcrumb from "@/utils/breadcrumb";
import { onMounted, ref, computed } from "vue";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";

const route = useRoute();
const user = ref(null);

breadcrumb([
  { path: "admin_customer_list", name: "Danh sách khách hàng" },
  { path: "admin_branch_update", name: route.query.name },
]);

const loading_data = ref(true);
const loading_active = ref(false);
const branches = ref(null);

const active_now = async () => {
  loading_active.value = true;
  try {
    const response = await axios.put("/admin/active-user/", null, {
      params: {
        id: route.query.id,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });
    if (response.data.success) {
      toast.success(response.data.message);
      user.value.is_verified = true;
    } else toast.error(response.data.message);
  } finally {
    loading_active.value = false;
  }
};

const lock_user_now = async () => {
  try {
    const response = await axios.put("/admin/lock-user", null, {
      params: {
        id: route.query.id,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });

    if (response.data.success) {
      toast.success(response.data.message);
      user.value.is_locked = !user.value.is_locked;
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {}
};

const fetchData = async () => {
  try {
    loading_data.value = true;
    const response = await axios.get("/admin/get-user-by-id", {
      params: {
        id: route.query.id,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });

    if (response.data.success) {
      user.value = response.data.data;
      console.log(user.value);
    } else toast.error(response.data.message);
  } finally {
    loading_data.value = false;
  }
};

const fetchBranches = async () => {
  try {
    const response = await axios.get("/branch");
    if (response.data.success) {
      branches.value = response.data.data;
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {}
};

onMounted(() => {
  fetchData();
  fetchBranches();
});
</script>
