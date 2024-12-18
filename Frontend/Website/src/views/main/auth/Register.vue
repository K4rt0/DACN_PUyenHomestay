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
              <!-- <div class="col-12 text-start text-light">
                <button type="button" class="border-secondary bg-white rounded p-2 w-100 mb-1">
                  <div class="d-flex align-items-center justify-content-center">
                    <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="30" height="30" viewBox="0 0 48 48">
                      <path
                        fill="#FFC107"
                        d="M43.611,20.083H42V20H24v8h11.303c-1.649,4.657-6.08,8-11.303,8c-6.627,0-12-5.373-12-12c0-6.627,5.373-12,12-12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C12.955,4,4,12.955,4,24c0,11.045,8.955,20,20,20c11.045,0,20-8.955,20-20C44,22.659,43.862,21.35,43.611,20.083z"
                      ></path>
                      <path fill="#FF3D00" d="M6.306,14.691l6.571,4.819C14.655,15.108,18.961,12,24,12c3.059,0,5.842,1.154,7.961,3.039l5.657-5.657C34.046,6.053,29.268,4,24,4C16.318,4,9.656,8.337,6.306,14.691z"></path>
                      <path fill="#4CAF50" d="M24,44c5.166,0,9.86-1.977,13.409-5.192l-6.19-5.238C29.211,35.091,26.715,36,24,36c-5.202,0-9.619-3.317-11.283-7.946l-6.522,5.025C9.505,39.556,16.227,44,24,44z"></path>
                      <path fill="#1976D2" d="M43.611,20.083H42V20H24v8h11.303c-0.792,2.237-2.231,4.166-4.087,5.571c0.001-0.001,0.002-0.001,0.003-0.002l6.19,5.238C36.971,39.205,44,34,44,24C44,22.659,43.862,21.35,43.611,20.083z"></path>
                    </svg>
                    <p class="text-dark fs-6 fw-bold m-0 ms-2">Tiếp tục với Google</p>
                  </div>
                </button>
              </div>
              <div class="col-12 text-start text-light">
                <div class="divider">
                  <div class="divider-text">hoặc</div>
                </div>
              </div> -->
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
        router.push({ name: "main_login" });
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
