<template>
  <div class="hero full-height bg-dark">
    <div class="container" style="margin-top: 100px">
      <div class="d-flex flex-row-reverse mb-3">
        <button @click="cancel_booking" v-if="booking?.status == 0 || booking?.status == 1 || booking?.status == 3" class="btn btn-md btn-danger">Huỷ đặt phòng</button>
      </div>

      <div class="card">
        <div class="card-body p-4">
          <h2 class="mb-0">
            Đơn đặt phòng của bạn đã <span :class="get_reservation_status(booking?.status).color">{{ get_reservation_status(booking?.status).message }}</span
            >.
          </h2>

          <div class="row m-0 mt-3 p-3 text-center rounded" style="background-color: rgb(228, 228, 228)">
            <div class="col-2 border-end border-dark">
              <h6 class="text-muted">CHECK-IN</h6>
              <h5 class="mb-0">{{ booking?.check_in }}</h5>
            </div>
            <div class="col-2 border-end border-dark">
              <h6 class="text-muted">CHECK-OUT</h6>
              <h5 class="mb-0">{{ booking?.check_out }}</h5>
            </div>
            <div class="col-2 border-end border-dark">
              <h6 class="text-muted">SỐ ĐÊM</h6>
              <h5 class="mb-0">{{ calculateNights(booking?.check_in, booking?.check_out) }}</h5>
            </div>
            <div class="col-2 border-end border-dark">
              <h6 class="text-muted">SỐ LƯỢNG KHÁCH</h6>
              <h5 class="mb-0">{{ booking?.guest_amount }}</h5>
            </div>
            <div class="col-2 border-end border-dark">
              <h6 class="text-muted">KHÁCH</h6>
              <h5 class="mb-0">{{ booking?.first_name + " " + booking?.last_name }}</h5>
            </div>
            <div class="col-2">
              <h6 class="fw-bold">TỔNG TIỀN</h6>
              <h5 class="mb-0 text-success fw-bold">{{ booking?.total_price.toLocaleString("vi-VN", { style: "currency", currency: "VND" }) }}</h5>
            </div>
          </div>
        </div>
      </div>
      <div class="card mt-3">
        <div class="card-body p-4">
          <div class="row m-0 mt-3 p-3 text-center rounded" style="background-color: rgb(228, 228, 228)">
            <div class="col-3 p-0 m-0">
              <img style="object-fit: cover" width="300px" height="200" :src="'https://i.imgur.com/' + booking?.room?.room_images[0].url + booking?.room?.room_images[0].type" alt="" />
            </div>
            <div class="col-9 text-start ps-3">
              <h3>{{ booking?.room?.branch?.name }}</h3>
              <h6 class="text-muted"><i class="fa-solid fa-location-dot me-2"></i>Địa chỉ: {{ booking?.room?.branch?.address }}, {{ booking?.room?.branch?.ward }}, {{ booking?.room?.branch?.district }}, {{ booking?.room?.branch?.province }}</h6>
              <h6 class="text-muted"><i class="fa-solid fa-bed me-2"></i>Hạng phòng: {{ booking?.room?.room_name }}</h6>
              <div class="row text-dark">
                <div v-for="facility in booking?.room?.facilities_rooms" class="col-3"><i :class="[facility.facility?.icon, 'me-2']"></i>{{ facility.facility?.name }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { useRoute } from "vue-router";
import { onMounted, ref } from "vue";
import axios from "@/utils/axios";
import { toast } from "vue-sonner";

const route = useRoute().params;
const booking = ref(null);

const calculateNights = (checkIn, checkOut) => {
  const checkInDate = new Date(checkIn);
  const checkOutDate = new Date(checkOut);
  const timeDifference = checkOutDate - checkInDate;
  const nights = timeDifference / (1000 * 3600 * 24);
  return nights;
};

const get_reservation_status = (status) => {
  switch (status) {
    case 0:
      return {
        color: "text-warning",
        message: "đang xem xét",
      };
    case 1:
      return {
        color: "text-success",
        message: "đã chấp nhận",
      };
    case (2, 4):
      return {
        color: "text-danger",
        message: "đã huỷ",
      };
    case 3:
      return {
        color: "text-success",
        message: "đã đặt",
      };
    case 5:
      return {
        color: "text-success",
        message: "đã hoàn thành",
      };
    default:
      return {
        color: "text-danger",
        message: "không xác định",
      };
  }
};

const cancel_booking = async () => {
  try {
    const response = await axios.put(`/booking/cancel`, null, {
      params: {
        booking_id: booking.value.booking_id,
      },
    });

    if (response.data.success) {
      toast.success(response.data.message);
      fetchBooking();
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {}
};

const fetchBooking = async () => {
  try {
    const response = await axios.get(`/booking/${route.id}`);

    if (response.data.success) {
      booking.value = response.data.data;
      console.log(booking.value);
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {}
};

onMounted(() => {
  fetchBooking();
});
</script>
