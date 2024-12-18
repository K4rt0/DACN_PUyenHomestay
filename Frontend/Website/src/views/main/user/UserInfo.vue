<template>
  <div class="hero home-search bg-dark" style="padding-top: 5.5rem">
    <div class="container h-100 d-flex justify-content-center align-items-center">
      <div v-if="loading" class="spinner-border"></div>
      <div v-else class="card w-100">
        <div class="card-body">
          <!-- Tab headers -->
          <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
              <button class="nav-link text-dark active" id="user-info-tab" data-bs-toggle="tab" data-bs-target="#user-info" type="button" role="tab" aria-controls="user-info" aria-selected="true">Thông tin cá nhân</button>
            </li>
            <li class="nav-item" role="presentation">
              <button class="nav-link text-dark" id="user-password-tab" data-bs-toggle="tab" data-bs-target="#user-password" type="button" role="tab" aria-controls="user-password" aria-selected="false">Mật khẩu</button>
            </li>
            <li class="nav-item" role="presentation">
              <button class="nav-link text-dark" id="user-booking-tab" data-bs-toggle="tab" data-bs-target="#user-booking" type="button" role="tab" aria-controls="user-booking" aria-selected="false">Đơn đặt phòng</button>
            </li>
          </ul>

          <!-- Tab content -->
          <div class="tab-content" id="myTabContent">
            <div class="tab-pane fade show active" id="user-info" role="tabpanel" aria-labelledby="tab-a">
              <form class="p-3" @submit.prevent="submit_change">
                <div class="row">
                  <div class="col-12">
                    <div class="form-group mb-3">
                      <label class="text-muted" for="full-name">Họ và tên</label>
                      <input type="text" class="form-control" id="full-name" v-model="profile.full_name" />
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="form-group mb-3">
                      <label class="text-muted" for="email">Email</label>
                      <input type="text" disabled class="form-control" id="email" v-model="profile.email" />
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="form-group mb-3">
                      <label class="text-muted" for="phone">Số điện thoại</label>
                      <input type="text" class="form-control" id="phone" v-model="profile.phone_number" />
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="form-group mb-3">
                      <label class="text-muted" for="is-verified">Xác thực tài khoản</label>
                      <input v-if="profile.is_verified" type="text" disabled class="form-control border-success text-success" id="is-verified" value="Đã xác thực" />
                      <div v-else class="row">
                        <div class="col-7 pe-1">
                          <input type="text" disabled class="form-control" :class="profile.is_verified ? 'border-success text-success' : 'border-danger text-danger'" id="is-verified" :value="profile.is_verified ? 'Đã xác thực' : 'Chưa xác thực'" />
                        </div>
                        <div class="col-5 ps-1">
                          <button type="button" @click="verify_action" class="btn h-100 w-100 btn-success">Xác thực ngay</button>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="form-group mb-3">
                      <label class="text-muted" for="reward-point">Điểm thưởng</label>
                      <input type="text" class="form-control" disabled id="reward-point" :value="profile.reward_points + ' điểm'" />
                    </div>
                  </div>
                  <div class="text-end">
                    <button type="button" class="btn btn-secondary me-2">Trở lại</button>
                    <button type="submit" class="btn btn-primary">Lưu thông tin</button>
                  </div>
                </div>
              </form>
              <div class="row">
                <!-- <div class="col-12 col-md-4 col-lg-4 mb-2">
                  <div class="card">
                    <div class="card-body">
                      <h3 class="text-center py-2">Đổi mật khẩu</h3>
                      <form class="p-3" @submit.prevent="submit_change_pwd">
                        <div class="form-group mb-3">
                          <label class="text-muted" for="old-password">Mật khẩu cũ</label>
                          <input type="password" class="form-control fs-4" id="old-password" placeholder="························" v-model="old_password" />
                        </div>
                        <div class="form-group mb-3">
                          <label class="text-muted" for="new_password">Mật khẩu</label>
                          <input type="password" class="form-control fs-4" id="new_password" placeholder="························" v-model="new_password" />
                        </div>
                        <div class="form-group mb-3">
                          <label class="text-muted" for="retype_new_password">Nhập lại mật khẩu</label>
                          <input type="password" class="form-control fs-4" id="retype_new_password" placeholder="························" v-model="retype_new_password" />
                        </div>
                        <div class="text-center">
                          <button type="submit" class="btn btn-success">Cập nhật</button>
                        </div>
                      </form>
                    </div>
                  </div>
                </div> -->
                <!-- <div class="col-12 col-md-8 col-lg-8">
                  <div class="card">
                    <div class="card-body">
                      
                    </div>
                  </div>
                </div> -->
              </div>
            </div>
            <div class="tab-pane fade" id="user-password" role="tabpanel" aria-labelledby="user-password-tab">
              <form class="p-3" @submit.prevent="submit_change_pwd">
                <div class="form-group mb-3">
                  <label class="text-muted" for="old-password">Mật khẩu cũ</label>
                  <input type="password" class="form-control fs-4" id="old-password" placeholder="························" v-model="old_password" />
                </div>
                <div class="form-group mb-3">
                  <label class="text-muted" for="new_password">Mật khẩu</label>
                  <input type="password" class="form-control fs-4" id="new_password" placeholder="························" v-model="new_password" />
                </div>
                <div class="form-group mb-3">
                  <label class="text-muted" for="retype_new_password">Nhập lại mật khẩu</label>
                  <input type="password" class="form-control fs-4" id="retype_new_password" placeholder="························" v-model="retype_new_password" />
                </div>
                <div class="text-center">
                  <button type="submit" class="btn btn-success">Cập nhật</button>
                </div>
              </form>
            </div>
            <div class="tab-pane fade" id="user-booking" role="tabpanel" aria-labelledby="user-booking-tab">
              <h5 class="my-3 ms-3">Danh sách phòng đã đặt</h5>
              <div class="card" v-for="(reservation, index) in profile.reservations" :key="reservation.booking_id" :class="{ 'mb-3': index !== profile.reservations.length - 1 }">
                <div class="row">
                  <div class="col-12 col-md-12 col-lg-8">
                    <div class="d-flex align-items-center">
                      <img :src="'https://i.imgur.com/' + reservation.room.room_images[0].url + reservation.room.room_images[0].type" style="height: 200px" alt="" class="img-fluid me-3" />
                      <div class="form-group mb-0">
                        <p class="mb-3 text-dark">Booking ID: {{ reservation.booking_id }}</p>
                        <div class="form-group mb-0">
                          <h5>
                            {{ reservation.room.branch.name }} - {{ reservation.room.room_name }} - <span :class="get_reservation_status(reservation.status).color">{{ get_reservation_status(reservation.status).message }}</span>
                          </h5>
                          <div class="row w-75">
                            <div class="col-5">
                              <p class="text-dark mb-0">Check-in</p>
                              <p class="text-dark fw-bold">{{ reservation.check_in }}</p>
                            </div>
                            <div class="col-2">
                              <p class="text-dark">•</p>
                            </div>
                            <div class="col-5">
                              <p class="text-dark mb-0">Check-out</p>
                              <p class="text-dark fw-bold">{{ reservation.check_out }}</p>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="col-12 col-md-12 col-lg-4" v-if="get_reservation_status(reservation.status).rate">
                    <div class="d-flex align-items-center h-100">
                      <div class="form-group mb-0 w-100">
                        <div class="text-end me-3">
                          <div class="d-flex align-items-center mb-2">
                            <div class="stars ms-0">
                              <label class="rate" v-for="star in 5" :key="`star-${reservation.booking_id}-${star}`" @click="setRating(index, star)">
                                <input type="radio" :name="'rate-' + reservation.booking_id" :id="'star-' + star + '-' + reservation.booking_id" :value="star" v-model="reservation.rating" />
                                <i :class="getStarClass(index, star)" class="star"></i>
                              </label>
                            </div>
                            <button @click="save_comment(reservation)" :disabled="reservation.is_commented" class="btn btn-secondary h-50">Lưu</button>
                          </div>
                          <input type="text" class="form-control mb-2" :disabled="reservation.is_commented" v-model="reservation.comment" placeholder="Đánh giá tại đây" />
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import axios from "@/utils/axios";
import { toast } from "vue-sonner";

const profile = ref(null);
const loading = ref(true);
const old_password = ref("");
const new_password = ref("");
const retype_new_password = ref("");

const verify_action = async () => {
  try {
    const response = await axios.post("/user/send-verification-email", null, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
      },
    });
    if (response.data.success) {
      toast.success(response.data.message);
      fetchProfile();
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {
    console.log(error);
  }
};
const submit_change_pwd = async () => {
  if (!old_password.value || !new_password.value || !retype_new_password.value) toast.error("Vui lòng điền đầy đủ thông tin");
  else if (new_password.value !== retype_new_password.value) toast.error("Mật khẩu không khớp");
  else if (new_password.value.length < 6) toast.error("Mật khẩu phải có ít nhất 6 ký tự");
  else if (new_password.value === old_password.value) toast.error("Mật khẩu mới không được trùng với mật khẩu cũ");
  else {
    const response = await axios.put("/user/change-password", null, {
      params: {
        old_password: old_password.value,
        new_password: new_password.value,
        retype_new_password: retype_new_password.value,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
      },
    });

    if (response.data.success) {
      toast.success(response.data.message);
      old_password.value = "";
      new_password.value = "";
      retype_new_password.value = "";
    } else {
      toast.error(response.data.message);
    }
  }
};
const submit_change = async () => {
  if (!profile.value.full_name || !profile.value.phone_number) toast.error("Vui lòng điền đầy đủ thông tin");
  else {
    const response = await axios.put("/user/update-profile", null, {
      params: {
        full_name: profile.value.full_name,
        phone_number: profile.value.phone_number,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
      },
    });

    if (response.data.success) {
      toast.success(response.data.message);
      old_password.value = "";
      new_password.value = "";
      retype_new_password.value = "";
    } else {
      toast.error(response.data.message);
    }
  }
};
const get_reservation_status = (status) => {
  switch (status) {
    case 0:
      return {
        color: "text-warning",
        message: "Đang xem xét",
        rate: false,
      };
    case 1:
      return {
        color: "text-success",
        message: "Đã chấp nhận",
        rate: false,
      };
    case (2, 4):
      return {
        color: "text-danger",
        message: "Đã huỷ",
        rate: false,
      };
    case 3:
      return {
        color: "text-success",
        message: "Đã đặt",
        rate: false,
      };
    case 5:
      return {
        color: "text-success",
        message: "Đã hoàn thành",
        rate: true,
      };
    default:
      return {
        color: "text-danger",
        message: "Không xác định",
      };
  }
};
const setRating = (index, rating) => {
  profile.value.reservations[index].rating = rating;
};
const getStarClass = (index, star) => {
  return profile.value.reservations[index].rating >= star ? "fas fa-star" : "far fa-star";
};
const save_comment = async (reservation) => {
  if (reservation.is_commented) toast.warning("Đã có lỗi xảy ra !");
  else {
    if (!reservation.rating) {
      toast.error("Vui lòng chọn số sao");
      return;
    }
    try {
      const response = await axios.put("/reservation/leave-comment", null, {
        params: {
          booking_id: reservation.booking_id,
          rate: parseInt(reservation.rating),
          comment: reservation.comment != null ? reservation.comment : "",
        },
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
        },
      });

      if (response.data.success) {
        toast.success("Đánh giá của bạn đã được lưu lại");
        reservation.is_commented = true;
      } else {
        toast.error(response.data.message);
      }
    } catch (error) {
      console.log(error);
    }
  }
};

const fetchProfile = async () => {
  try {
    const response = await axios.get("/user/profile", {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
      },
    });
    if (response.data.success) {
      profile.value = response.data.data;
      profile.value.reservations.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
      profile.value.reservations.forEach((reservation, index) => {
        if (reservation.rating) {
          setRating(index, reservation.rating);
          reservation.is_commented = true;
        } else reservation.is_commented = false;
      });
      loading.value = false;
    } else {
      toast.error(response.data.message);
    }
  } catch (error) {
    console.log(error);
  }
};

onMounted(() => {
  fetchProfile();
});
</script>

<style scoped>
@import "/src/assets/main/css/rating.css";
</style>
