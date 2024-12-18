<template>
  <div class="hero full-height bg-dark">
    <div class="container h-100">
      <div class="d-flex flex-column align-items-center justify-content-center h-100">
        <i :class="icon + ' ' + color" class="mb-3" style="font-size: 200px"></i>
        <h1 :class="color">{{ message }}</h1>
        <a href="/" class="btn btn-outline-light mt-3">Quay lại trang chủ</a>
      </div>
    </div>
  </div>
</template>
<script setup>
import axios from "@/utils/axios";
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";

const icon = ref("fa-regular fa-face-thinking");
const color = ref("text-warning");
const message = ref("Đang xác thực tài khoản...");

const route = useRoute();

const verify = async () => {
  try {
    const response = await axios.get("/user/verify-email", {
      params: {
        jwt_token: route.query.token,
      },
    });

    if (response.data.success) {
      icon.value = response.data.data.icon;
      color.value = response.data.data.color;
      message.value = response.data.message;
    }
  } catch (error) {
    console.log(error);
  }
};

onMounted(() => {
  if (route.query.token) {
    verify();
  }
});
</script>
