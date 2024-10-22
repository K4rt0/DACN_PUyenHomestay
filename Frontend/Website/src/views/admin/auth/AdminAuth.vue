<template>
  <div class="card px-sm-6 px-0">
    <div class="card-body">
      <!-- Logo -->
      <div class="app-brand justify-content-center">
        <a href="index.html" class="app-brand-link gap-2">
          <span class="app-brand-text demo text-heading fw-bold">Admin Dashboard</span>
        </a>
      </div>
      <form id="formAuthentication" class="mb-6" @submit.prevent="login()">
        <div class="mb-6">
          <label for="email" class="form-label">Email address</label>
          <input v-model="email" type="text" class="form-control" id="email" name="email-username" placeholder="example@email.com" autofocus="" />
        </div>
        <div class="mb-6 form-password-toggle">
          <label class="form-label" for="password">Password</label>
          <div class="input-group input-group-merge">
            <input v-model="password" type="password" id="password" class="form-control" name="password" placeholder="&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;&#xb7;" aria-describedby="password" />
            <span class="input-group-text cursor-pointer"><i class="bx bx-hide"></i></span>
          </div>
        </div>
        <div class="mb-4">
          <div class="d-flex justify-content-between mt-4">
            <div class="form-check mb-0 ms-2">
              <input class="form-check-input" type="checkbox" id="remember-me" />
              <label class="form-check-label" for="remember-me"> Remember Me </label>
            </div>
          </div>
        </div>
        <button v-if="!loading" class="btn btn-primary d-grid w-100" type="submit">Đăng nhập</button>
        <button v-else class="btn btn-primary d-grid w-100" disabled type="submit">
          <div class="spinner-border border-4 mt-1" style="width: 20px; height: 20px"></div>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { toast } from "vue-sonner";
import axios from "@/utils/axios";
import router from "@/router";

let email = ref("");
let password = ref("");
let loading = ref(false);

function login() {
  loading.value = true;
  if (email.value === "" || password.value === "") {
    loading.value = false;
    toast.warning("Tài khoản hoặc mật khẩu không được bỏ trống !");
  } else {
    const formData = new FormData();
    formData.append("email", email.value);
    formData.append("password", password.value);

    axios
      .post("/admin/login", formData)
      .then((response) => {
        const res = response.data;
        if (res.success) {
          toast.success("Đăng nhập thành công !");
          localStorage.setItem("jwt_admin_token", res.data.token);
          localStorage.setItem("admin_user_data", JSON.stringify(res.data));
          setTimeout(() => {
            router.push({ name: "admin_dashboard" });
          }, 1000);
        } else {
          toast.warning(res.message);
        }
        loading.value = false;
      })
      .catch(() => {
        loading.value = false;
      });
  }
}
</script>
