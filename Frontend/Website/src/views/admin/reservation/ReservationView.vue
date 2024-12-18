<template>
  <div v-if="loading" class="d-flex flex-column align-items-center" style="padding: 100px 0">
    <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
    <h3 class="m-0">Đang tải dữ liệu...</h3>
  </div>
  <div v-else>
    <div class="d-flex flex-row-reverse justify-content-between mb-5">
      <div class="form-group">
        <button type="button" @click="reservation_action('cancel')" v-if="reservation?.status == 0" class="btn btn-sm btn-danger me-1">Từ chối</button>
        <button type="button" @click="reservation_action('accept')" v-if="reservation?.status == 0" class="btn btn-sm btn-primary">Chấp nhận</button>
        <button type="button" @click="reservation_action('cancel')" v-if="reservation?.status == 1 || reservation?.status == 3" class="btn btn-sm btn-danger">Huỷ đặt phòng</button>
        <button type="button" @click="reservation_action('done')" v-if="reservation?.status == 1 || reservation?.status == 3" class="btn btn-sm btn-primary ms-2">Hoàn thành</button>
      </div>
      <router-link :to="{ name: 'admin_reservation_list' }" class="btn btn-sm btn-secondary">Trở về</router-link>
    </div>
    <div class="card mb-6">
      <div class="card-header p-0 nav-align-top">
        <ul class="nav nav-tabs" role="tablist">
          <li class="nav-item" role="presentation">
            <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#reservation-detail" role="tab" aria-selected="true">Thông tin đặt phòng</button>
          </li>
          <li class="nav-item" role="presentation">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#reservation-payment" role="tab" aria-selected="false" tabindex="-1">Thông tin thanh toán</button>
          </li>
          <li class="nav-item" role="presentation">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#reservation-room" role="tab" aria-selected="false" tabindex="-1">Chi nhánh - phòng</button>
          </li>
          <li class="nav-item" role="presentation">
            <button class="nav-link" data-bs-toggle="tab" data-bs-target="#reservation-review" role="tab" aria-selected="false" tabindex="-1">Đánh giá</button>
          </li>
        </ul>
      </div>

      <div class="tab-content">
        <div class="tab-pane fade active show" id="reservation-detail" role="tabpanel">
          <div class="row">
            <div class="col-12">
              <h4 class="text-center text-uppercase">Thông tin nhận phòng</h4>
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="first-name">Họ</label>
              <input type="text" class="form-control" id="first-name" disabled :value="reservation?.first_name" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="last-name">Tên</label>
              <input type="text" class="form-control" id="last-name" disabled :value="reservation?.last_name" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="phone">Số điện thoại</label>
              <input type="text" class="form-control" id="phone" disabled :value="reservation?.phone" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="check-in">Ngày nhận phòng</label>
              <input type="text" class="form-control" id="check-in" disabled :value="reservation?.check_in" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="check-out">Ngày trả phòng</label>
              <input type="text" class="form-control" id="check-out" disabled :value="reservation?.check_in" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="adults">Số khách</label>
              <input type="text" class="form-control" id="adults" disabled :value="reservation?.guest_amount + ' người'" />
            </div>
            <div class="col-6">
              <label class="form-label" for="reservation-created-date">Điểm sử dụng</label>
              <input type="text" class="form-control" id="reservation-point-used" disabled :value="reservation?.reward_points + ' điểm'" />
            </div>
            <div class="col-6">
              <label class="form-label" for="reservation-created-date">Thời gian đặt phòng cụ thể</label>
              <input type="text" class="form-control" id="reservation-created-date" disabled :value="reservation?.created_at.replace('T', ' ')" />
            </div>
          </div>
          <hr />
          <div class="row">
            <div class="col-12">
              <h4 class="text-center text-uppercase">Tài khoản đặt phòng</h4>
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="full-name">Họ tên</label>
              <input type="text" class="form-control" id="full-name" disabled :value="reservation?.user.full_name" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="identifier">CCCD/CMND</label>
              <input type="text" class="form-control" id="identifier" disabled :value="reservation?.user.identifier" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="user-email">Email</label>
              <input type="text" class="form-control" id="user-email" disabled :value="reservation?.user.email" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="user-phone">Số điện thoại</label>
              <input type="text" class="form-control" id="user-phone" disabled :value="reservation?.user.phone_number" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="is-blocked">Cấm</label>
              <input type="text" class="form-control" :class="reservation?.user.is_locked ? 'border-danger text-danger' : 'border-success text-success'" id="is-blocked" disabled :value="reservation?.user.is_locked ? 'bị cấm' : 'không bị cấm'" />
            </div>
            <div class="col-4 mb-3">
              <label class="form-label" for="is_verified">Xác thực</label>
              <input type="text" class="form-control" :class="reservation?.user.is_verified ? 'border-success text-success' : 'border-warning text-warning'" id="is_verified" disabled :value="reservation?.user.is_verified ? 'đã xác thực' : 'chưa xác thực'" />
            </div>
            <div class="col-12">
              <label class="form-label" for="user-created-date">Ngày tạo tài khoản</label>
              <input type="text" class="form-control" id="user-created-date" disabled :value="reservation?.created_at.replace('T', ' ')" />
            </div>
          </div>
        </div>
        <div class="tab-pane fade" id="reservation-payment" role="tabpanel">
          <div class="row">
            <div class="col-6 mb-3">
              <label class="form-label" for="full-name">Hình thức thanh toán</label>
              <input type="text" class="form-control text-white" id="full-name" disabled :value="reservation?.payment_method == 0 ? 'Thanh toán tại quầy' : reservation?.payment_method == 1 ? 'Thanh toán VNPay' : 'Thanh toán MoMo'" />
            </div>
            <div class="col-6 mb-3">
              <label class="form-label" for="identifier">Trạng thái thanh toán</label>
              <input type="text" class="form-control" :class="reservation?.payment_status ? 'text-success' : 'text-warning border-warning'" id="identifier" disabled :value="reservation?.payment_status ? 'Đã thanh toán' : 'Chưa thanh toán'" />
            </div>
            <div class="col-12 mb-3">
              <label class="form-label" for="identifier">Tổng tiền</label>
              <input type="text" class="form-control" id="total_price" disabled :value="reservation?.total_price.toLocaleString('vi-VN') + 'đ'" />
            </div>
          </div>
        </div>
        <div class="tab-pane fade" id="reservation-room" role="tabpanel">
          <div class="row">
            <div class="col-4">
              <div class="card">
                <div class="card-header py-2">
                  <h4 class="mb-0">Phòng</h4>
                </div>
                <hr class="m-0" />
                <div class="card-body">
                  <img class="mb-3" :src="'https://i.imgur.com/' + reservation?.room?.room_images[0]?.url + reservation?.room?.room_images[0]?.type" width="100%" height="250px" alt="" />
                  <h4 class="fw-bold mb-0">{{ reservation?.room?.branch.name }}</h4>
                  <h6 class="mb-0">Hạng phòng: {{ reservation?.room?.room_name }}</h6>
                  <h6 class="mb-0">Số phòng: {{ reservation?.room?.room_number }}</h6>
                  <h6 class="mb-0">
                    Giá tiền: <span class="text-success">{{ reservation?.room?.cost }}/đêm</span>
                  </h6>
                </div>
              </div>
            </div>
            <div class="col-8">
              <div class="card">
                <div class="card-header py-2">
                  <h4 class="mb-0">Tiện nghi</h4>
                </div>
                <hr class="m-0" />
                <div class="card-body">
                  <div class="row">
                    <div class="col-3" v-for="facility in reservation?.room?.facilities_rooms">
                      <div class="card">
                        <div class="card-body">
                          <p class="m-0">
                            <i :class="facility.facility.icon" class="me-2"></i>
                            {{ facility.facility.name }}
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="tab-pane fade" id="reservation-review" role="tabpanel">
          <div class="stars ms-0">
            <label class="rate" v-for="star in 5" :key="`star-${reservation?.booking_id}-${star}`">
              <input type="radio" :name="'rate-' + reservation?.booking_id" :id="'star-' + star + '-' + reservation?.booking_id" :value="star" />
              <i :class="getStarClass(star)" class="star"></i>
            </label>
            <p class="mb-0">Lời nhắn: {{ reservation?.comment }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";
import axios from "@/utils/axios";
import breadcrumb from "@/utils/breadcrumb";
import { useRoute } from "vue-router";
import formatCurrencyVND from "@/utils/formatCurrencyVND";
import { toast } from "vue-sonner";

const route = useRoute();
const loading = ref(true);

breadcrumb([
  { path: "admin_reservation_list", name: "Danh sách đặt phòng" },
  { path: "admin_reservation_list", name: route.query.id },
]);

const reservation = ref(null);

const reservation_action = async (action) => {
  if (action == "accept" || action == "cancel" || action == "done") {
    try {
      const response = await axios.put("/reservation", null, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
        params: {
          id: route.query.id,
          action: action,
        },
      });

      if (response.data.success) {
        toast.success(response.data.message);
        fetchData();
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {
      console.error(error);
    }
  }
};
const getStarClass = (star) => {
  return reservation.value.rating >= star ? "fas fa-star" : "far fa-star";
};
const fetchData = async () => {
  try {
    const response = await axios.get("/reservation/get-by-id", {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
      params: {
        id: route.query.id,
      },
    });

    if (response.data.success) {
      loading.value = false;
      reservation.value = response.data.data;
      reservation.value.room.cost = formatCurrencyVND(reservation.value.room.cost);
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {
    console.error(error);
  }
};

onMounted(() => {
  fetchData();
});
</script>

<style scoped>
@import "/src/assets/main/css/rating.css";
</style>
