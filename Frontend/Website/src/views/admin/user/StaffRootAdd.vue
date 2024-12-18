<template>
  <div class="card">
    <div class="card-header">
      <div class="d-flex justify-content-between">
        <h4 class="m-0">Thêm quản trị viên</h4>
        <div class="form-group">
          <router-link :to="{ name: 'admin_staff_list' }" class="btn btn-sm btn-secondary me-1">Trở về</router-link>
        </div>
      </div>
    </div>
    <div v-if="user == null" class="card-body">
      <div class="row g-6">
        <div class="col-12 col-md-12 col-lg-11">
          <label for="search-email" class="form-label">Địa chỉ email</label>
          <input id="search-email" v-model="find_email" type="text" class="form-control" placeholder="example@email.com" />
        </div>
        <div class="col-12 col-md-12 col-lg-1 d-flex align-items-end">
          <button v-if="!find_loading" type="button" @click="find_user" class="btn btn-primary w-100">Tìm</button>
          <button v-else type="button" disabled @click="find_user" class="btn btn-primary w-100">
            <div class="spinner-border" style="font-size: 20px"></div>
          </button>
        </div>
      </div>
    </div>
    <div v-else class="card-body">
      <div class="row">
        <div class="col-12 col-md-6 col-lg-6">
          <div class="row g-6">
            <div class="col-12">
              <label for="search-email" class="form-label">Địa chỉ email</label>
              <input id="search-email" v-model="find_email" type="text" class="form-control" placeholder="example@email.com" />
            </div>
            <div class="col-12 d-flex align-items-end">
              <button v-if="!find_loading" type="button" @click="find_user" class="btn btn-primary w-100">Tìm</button>
              <button v-else type="button" disabled @click="find_user" class="btn btn-primary w-100">
                <div class="spinner-border" style="font-size: 20px"></div>
              </button>
            </div>
          </div>
        </div>
        <div class="col-12 col-md-6 col-lg-6">
          <div class="row g-6">
            <div class="col-12">
              <label for="search-email" class="form-label">Họ tên</label>
              <input id="search-email" v-model="user.full_name" type="text" class="form-control" disabled />
            </div>
            <div class="col-12">
              <label for="search-email" class="form-label">Địa chỉ email</label>
              <input id="search-email" v-model="user.email" type="text" class="form-control" disabled />
            </div>
            <div class="col-12">
              <label for="search-email" class="form-label">Số điện thoại</label>
              <input id="search-email" v-model="user.phone_number" type="text" class="form-control" disabled />
            </div>
            <div class="col-12">
              <label for="select-branch" class="form-label">Chức vụ</label>
              <select id="select-branch" v-model="role_selected" class="form-select">
                <option selected disabled value="0">Chọn chức vụ</option>
                <option value="1">Nhân viên</option>
                <option value="2">Quản lý</option>
              </select>
            </div>
            <div class="col-12">
              <label for="branch" class="form-label">Chi nhánh</label>
              <select id="branch" v-model="branch_selected" @change="filterItems" class="form-select">
                <option value="0">Chọn chi nhánh</option>
                <option v-for="option in branch" :key="option.id" :value="option.id">{{ option.name }}</option>
              </select>
            </div>
            <div class="col-12 d-flex align-items-end">
              <button type="button" @click="add_role" class="btn btn-primary me-2 w-75">Xác nhận</button>
              <button type="button" class="btn btn-secondary w-50">Huỷ</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import axios from "@/utils/axios";
import { ref, onMounted } from "vue";
import { toast } from "vue-sonner";
import breadcrumb from "@/utils/breadcrumb";

breadcrumb([
  { path: "admin_staff_list", name: "Danh sách quản trị viên" },
  { path: "admin_dashboard", name: "Thêm mới" },
]);

const branch = ref(null);
const branch_selected = ref("0");
const role_selected = ref("0");
const loading = ref(false);
const find_email = ref("");
const find_loading = ref(false);
const user = ref(null);

const fetchBranch = async () => {
  loading.value = true;
  try {
    const response = await axios.get("/admin/get-branches");
    if (response.data.success) {
      branch.value = response.data.data;
      console.log(branch.value);
    } else toast.warning(response.data.message);
  } catch (error) {
    toast.error("Failed to fetch branch");
  } finally {
    loading.value = false;
  }
};

const find_user = async () => {
  find_loading.value = true;
  if (find_email.value === "") toast.warning("Vui lòng nhập email");
  else {
    try {
      const response = await axios.get("/user/get-by-email", {
        params: { email: find_email.value },
      });
      if (response.data.success) {
        if (response.data.data.role != 0) toast.warning("Người này đã có chức vụ trước đó !");
        else {
          user.value = response.data.data;
          console.log(user.value);
        }
      } else toast.warning(response.data.message);
    } finally {
      find_loading.value = false;
    }
  }
};

const add_role = async () => {
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

onMounted(() => {
  fetchBranch();
});
</script>
