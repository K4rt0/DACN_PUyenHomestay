<template>
  <div class="hero full-height bg-dark">
    <div class="wrapper d-flex align-items-center justify-content-center text-center">
      <div v-if="paymentSuccess == 'True' || paymentSuccess == 'true'" class="container">
        <i class="fa-regular fa-circle-check text-success" style="font-size: 15vh; font-weight: 200"></i>
        <p class="text-secondary fs-2 fw-bold mt-3">Đặt phòng thành công !</p>
        <p class="text-secondary fs-6 fw-semibold mt-2">Cảm ơn bạn đã tin tưởng và chọn chúng tôi để nghĩ dưỡng.</p>
        <p class="text-secondary fs-6 fw-semibold">Chúng tôi sẽ liên hệ với bạn sớm nhất để tiến hành xác thực thông tin đặt phòng.</p>
        <div class="d-flex justify-content-center">
          <a :href="booking_url" target="_blank" class="btn btn-dark border border-success p-1 mt-3 fw-semibold text-secondary" style="width: 250px">Chi tiết đơn đặt phòng <i class="fa-solid fa-caret-right"></i></a>
        </div>
      </div>
      <div v-else class="container">
        <i class="fa-regular fa-circle-xmark text-danger" style="font-size: 15vh; font-weight: 200"></i>
        <p class="text-secondary fs-2 fw-bold mt-3">Đặt phòng không thành công !</p>
        <p class="text-secondary fs-6 fw-semibold mt-2">Cảm ơn bạn đã tin tưởng và chọn chúng tôi để nghĩ dưỡng !</p>
        <p class="text-secondary fs-6 fw-semibold">Tuy nhiên có thể đã xảy ra lỗi khi thực hiện đặt phòng, vui lòng thử lại</p>
        <p class="text-secondary fs-6 fw-semibold">Hoặc liên hệ 0949.598.227 để được hỗ trợ trực tiếp</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";

const route = useRoute().query;
const paymentType = ref("");
const paymentSuccess = ref("");
const bid = ref("");
const booking_url = ref("");

onMounted(() => {
  const requiredParams = ["paymentType", "paymentSuccess"];
  const missingParams = requiredParams.filter((param) => !route.hasOwnProperty(param));
  if (missingParams.length > 0) window.location.href = "/";
  else {
    paymentType.value = route.paymentType;
    paymentSuccess.value = route.paymentSuccess;
    if (paymentSuccess.value == "True" || paymentSuccess.value == "true") {
      if (!route.hasOwnProperty("bid")) {
        window.location.href = "/";
      } else {
        bid.value = route.bid;
        booking_url.value = `/booking/${bid.value}`;
      }
    }
  }
});
</script>
