<template>
  <div class="hero full-height jarallax">
    <video class="jarallax-img position-absolute" loop muted autoplay>
      <source src="../../../assets/main/video/sunset.mp4" type="video/mp4" />
    </video>
    <div class="container-fluid h-100 d-flex justify-content-center align-items-center" style="background-color: rgba(0, 0, 0, 0.7)">
      <div class="card w-50" style="background-color: rgba(0, 0, 0, 0.7)">
        <div class="card-body px-5 py-4">
          <h4 class="card-title text-white text-center fw-bold mb-3">ĐĂNG KÝ</h4>
          <form @submit.prevent="submit_register">
            <div class="row">
              <div class="col-12 mb-3 text-start text-light">
                <label for="name" class="form-label fw-semibold">Họ và tên</label>
                <input type="text" class="form-control" id="name" v-model="name" placeholder="Nguyễn Văn A" />
              </div>
              <div class="col-12 mb-3 text-start text-light">
                <label for="email" class="form-label fw-semibold">Địa chỉ email</label>
                <input type="email" class="form-control" id="email" v-model="email" placeholder="example@email.com" />
              </div>
              <div class="col-12 col-md-6 col-lg-6 mb-3 text-start text-light">
                <label for="password" class="form-label fw-semibold">Mật khẩu</label>
                <input type="password" class="form-control" id="password" v-model="password" placeholder="············" />
              </div>
              <div class="col-12 col-md-6 col-lg-6 mb-4 text-start text-light">
                <label for="retype_password" class="form-label fw-semibold">Nhập lại mật khẩu</label>
                <input type="password" class="form-control" id="retype_password" v-model="retype_password" placeholder="············" />
              </div>
              <div class="col-12">
                <button type="submit" class="btn btn-success w-100 fw-semibold">Đăng ký</button>
              </div>
              <div class="col-12 mt-3 p-0 mb-0 text-center text-white">
                <p class="fs-6">Bạn đã có tài khoản? Đăng nhập <router-link :to="{ name: 'main_login' }">tại đây</router-link></p>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import router from "@/router";
import axios from "@/utils/axios";
import { ref, onMounted } from "vue";
import { toast } from "vue-sonner";

const name = ref("");
const email = ref("");
const password = ref("");
const retype_password = ref("");

const submit_register = async () => {
  if (validate()) {
    try {
      const response = await axios.post("/user/register", {
        full_name: name.value,
        email: email.value,
        password: password.value,
      });

      if (response.data.success) {
        toast.success(response.data.message);
        window.location.href = "/profile";
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {}
  }
};

const validate = () => {
  if (name.value === "") {
    toast.warning("Họ và tên không được để trống !");
    return false;
  }
  if (email.value === "") {
    toast.warning("Địa chỉ email không được để trống !");
    return false;
  }
  if (password.value === "") {
    toast.warning("Mật khẩu không được để trống !");
    return false;
  }
  if (retype_password.value === "") {
    toast.warning("Nhập lại mật khẩu không được để trống !");
    return false;
  }
  if (password.value !== retype_password.value) {
    toast.warning("Mật khẩu không khớp !");
    return false;
  }
  return true;
};

onMounted(() => {
  const script = document.createElement("script");
  script.src = "/src/assets/main/js/gsi.js";
  script.async = true;
  script.onload = () => {
    google.accounts.id.initialize({
      client_id: "868168887822-j7arobmbje43na6vu9ekh1ulcoohfvs7.apps.googleusercontent.com",
      callback: async function (response) {
        try {
          const res = await axios.get("/auth/google", {
            params: {
              access_token: response.credential,
            },
          });
          if (res.data.success) {
            toast.success(res.data.message);
            window.location.href = "/";
            localStorage.setItem("jwt_token_website", res.data.data.token);
          } else {
            toast.error(res.data.message);
          }
        } catch (error) {
          console.log(error);
        }
      },
    });
    google.accounts.id.prompt();
  };
  document.head.appendChild(script);
});
</script>
