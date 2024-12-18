<template>
  <div class="hero full-height jarallax">
    <video class="jarallax-img position-absolute" loop muted autoplay>
      <source src="../../../assets/main/video/sunset.mp4" type="video/mp4" />
    </video>
    <div class="container-fluid h-100 d-flex justify-content-center align-items-center" style="background-color: rgba(0, 0, 0, 0.7)">
      <div class="card w-50" style="background-color: rgba(0, 0, 0, 0.7)">
        <div class="card-body px-5 py-4">
          <h4 class="card-title text-white text-center fw-bold mb-3">ĐĂNG NHẬP</h4>
          <form @submit.prevent="submit_login">
            <div class="row">
              <div class="col-12 col-md-12 col-lg-12 mb-3 text-start text-light">
                <label for="email" class="form-label fw-semibold">Địa chỉ Email</label>
                <input type="email" class="form-control" v-model="email" id="email" placeholder="example@email.com" />
              </div>
              <div class="col-12 col-md-12 col-lg-12 mb-3 text-start text-light">
                <label for="password" class="form-label fw-semibold">Mật khẩu</label>
                <input type="password" class="form-control" v-model="password" id="password" placeholder="············" />
              </div>
              <div class="col-12 col-md-12 col-lg-12 ms-3 mb-4 text-start text-light form-check">
                <label class="form-check-label" for="remember-me">Nhớ đăng nhập của tôi</label>
                <input type="checkbox" class="form-check-input" v-model="remember_me" id="remember-me" />
              </div>
              <div class="col-12 col-md-12 col-lg-12">
                <button type="submit" class="btn btn-success w-100 fw-semibold">Đăng nhập</button>
              </div>
              <div class="col-12 mt-3 p-0 mb-0 text-center text-white">
                <p class="fs-6">Bạn chưa có tài khoản? Đăng ký <router-link :to="{ name: 'register' }">tại đây</router-link></p>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { toast } from "vue-sonner";
import axios from "@/utils/axios";

const email = ref("");
const password = ref("");
const remember_me = ref(false);

const submit_login = async () => {
  if (validate()) {
    try {
      const response = await axios.post("/user/login", null, {
        params: {
          email: email.value,
          password: password.value,
          remember: remember_me.value,
        },
      });

      if (response.data.success) {
        window.location.href = "/";

        localStorage.setItem("jwt_token_website", response.data.data.token);
        toast.success(response.data.message);
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {}
  }
};

const validate = () => {
  if (email.value === "") {
    toast.error("Vui lòng nhập địa chỉ email");
    return false;
  }

  if (password.value === "") {
    toast.error("Vui lòng nhập mật khẩu");
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
