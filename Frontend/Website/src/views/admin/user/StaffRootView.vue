<template>
  <div v-if="loading" class="d-flex flex-column align-items-center" style="padding: 100px 0">
    <div clasiv class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
    <h3 class="m-0">Đang tải dữ liệu...</h3>
  </div>
  <div v-else class="card">
    <div class="card-header">
      <div class="d-flex justify-content-between">
        <h4 class="m-0">Xem thông tin quản trị viên</h4>
        <div class="form-group">
          <router-link :to="{ name: 'admin_staff_list' }" class="btn btn-sm btn-secondary me-1">Trở về</router-link>
        </div>
      </div>
    </div>
    <div class="card-body">
      <div class="row g-6">
        <div class="col-6">
          <label for="search-email" class="form-label">Họ tên</label>
          <input id="search-email" :value="user?.full_name" type="text" class="form-control" disabled />
        </div>
        <div class="col-6">
          <label for="select-branch" class="form-label">Chức vụ</label>
          <select id="select-branch" v-model="role_selected" class="form-select">
            <option selected disabled value="0">Chọn chức vụ</option>
            <option value="1">Nhân viên</option>
            <option value="2">Quản lý</option>
          </select>
        </div>
        <div class="col-6">
          <label for="search-email" class="form-label">Địa chỉ email</label>
          <input id="search-email" :value="user?.email" type="text" class="form-control" disabled />
        </div>
        <div class="col-6">
          <label for="branch" class="form-label">Chi nhánh</label>
          <select id="branch" v-model="branch_selected" class="form-select">
            <option value="0">Chọn chi nhánh</option>
            <option v-for="option in branch" :key="option.id" :value="option.id">{{ option.name }}</option>
          </select>
        </div>
        <div class="col-6">
          <label for="search-email" class="form-label">Số điện thoại</label>
          <input id="search-email" :value="user?.phone_number" type="text" class="form-control" disabled />
        </div>
        <div class="col-6 d-flex align-items-end">
          <button type="button" @click="save_role" class="btn btn-primary me-2 w-50">Lưu</button>
          <button type="button" @click="take_role" class="btn btn-danger w-50">Huỷ chức vụ</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import axios from "@/utils/axios";
import { ref, onMounted } from "vue";
import { toast } from "vue-sonner";
import { useRoute } from "vue-router";
import breadcrumb from "@/utils/breadcrumb";

const route = useRoute();

breadcrumb([
  { path: "admin_staff_list", name: "Danh sách quản trị viên" },
  { path: "admin_dashboard", name: route.query.name },
]);

const branch = ref(null);
const branch_selected = ref("0");
const role_selected = ref("0");
const loading = ref(true);
const find_email = ref("");
const user = ref(null);

const fetchBranch = async () => {
  try {
    const response = await axios.get("/admin/get-branches");
    if (response.data.success) {
      branch.value = response.data.data;
    } else toast.warning(response.data.message);
  } catch (error) {
  } finally {
  }
};

const fetchUser = async () => {
  try {
    const response = await axios.get("/user/get-by-id", {
      params: { id: route.query.id },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });
    if (response.data.success) {
      console.log(response.data.data);
      user.value = response.data.data;
      branch_selected.value = user.value.branch.id;
      role_selected.value = user.value.role;
    } else toast.warning(response.data.message);
  } finally {
    loading.value = false;
  }
};

const save_role = async () => {
  if (role_selected.value === "0") toast.warning("Vui lòng chọn chức vụ");
  else if (branch_selected.value === "0") toast.warning("Vui lòng chọn chi nhánh");
  else {
    try {
      const response = await axios.put("/admin/update-role", null, {
        params: {
          id: user.value.id,
          role: role_selected.value,
          branch: branch_selected.value,
          type: "make",
          from: "root",
        },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });
      if (response.data.success) {
        toast.success(response.data.message);
        user.value = null;
        find_email.value = "";
        role_selected.value = "0";
        branch_selected.value = "0";

        window.location.href = "/admin/staff-list";
      } else toast.error(response.data.message);
    } catch (error) {
      toast.error("Failed to add role");
    }
  }
};
const take_role = async () => {
  try {
    const response = await axios.put("/admin/update-role", null, {
      params: {
        id: user.value.id,
        role: role_selected.value,
        branch: branch_selected.value,
        type: "take",
        from: "root",
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });
    if (response.data.success) {
      toast.success(response.data.message);
      user.value = null;
      find_email.value = "";
      role_selected.value = "0";
      branch_selected.value = "0";

      window.location.href = "/admin/staff-list";
    } else toast.error(response.data.message);
  } catch (error) {
    toast.error("Failed to add role");
  }
};

onMounted(() => {
  fetchBranch();
  fetchUser();
});
</script>
