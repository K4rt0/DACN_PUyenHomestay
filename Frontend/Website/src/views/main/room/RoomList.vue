<template>
  <main>
    <div class="hero home-search jarallax bg-secondary" style="height: 35vh; padding-top: 100px">
      <div class="wrapper opacity-mask d-flex align-items-center justify-content-center text-center animate_hero is-transitioned" data-opacity-mask="rgba(0, 0, 0, 0.5)">
        <div class="container mt-5">
          <div class="row g-0 booking_form">
            <div class="col-lg-4">
              <div class="form-group">
                <DatePicker class="form-control" v-model="date_booking" placeholder="Check in / Check out" readonly />
                <i class="bi bi-calendar2"></i>
              </div>
            </div>
            <div class="col-lg-3">
              <div class="nice-select wide mb-0 border-0 border-end" tabindex="0">
                <span class="current">{{ branch_selected.name }}</span>
                <ul class="list">
                  <li data-value="Chọn chi nhánh" class="option selected focus disabled">Chọn chi nhánh</li>
                  <li v-for="branch in branches" :key="branch.id" :data-value="branch.name" class="option" :class="{ selected: branch_selected.name === branch.name }" @click="branch_selected = branch">
                    {{ branch.name }}
                  </li>
                </ul>
              </div>
            </div>
            <div class="col-lg-3 ps-lg-0">
              <div class="qty-buttons">
                <label>Số lượng</label>
                <input type="button" value="+" class="qtyplus" @click="adults_booking++" name="adults" />
                <input type="text" name="adults" id="adults" v-model="adults_booking" class="qty form-control" />
                <input type="button" value="-" class="qtyminus" @click="if (adults_booking - 1 > 0) adults_booking--;" name="adults" />
              </div>
            </div>
            <div class="col-lg-2">
              <button v-if="searching" type="button" class="disabled text-center d-flex align-items-center justify-content-center">
                <div class="spinner-border" style="width: 20px; height: 20px"></div>
              </button>
              <button v-else type="button" @click="update_search_room" class="text-center">Cập nhật</button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="container p-5" style="position: relative; z-index: -1; padding-top: 100px !important">
      <div v-for="(room, index) in rooms" class="row_list_version_2" :class="{ inverted: (index + 1) % 2 == 0 }">
        <div class="row g-0 h-50 align-items-center">
          <div class="col-xl-8" :class="{ 'order-xl-2': (index + 1) % 2 == 0 }">
            <div class="owl-carousel owl-theme carousel_item_1 kenburns rounded-img ratio ratio-1x1">
              <div v-for="image in room.room_images" class="item">
                <img style="height: 90vh; object-fit: cover" :src="`https://i.imgur.com/${image.url}${image.type}`" alt="" />
              </div>
            </div>
          </div>
          <div class="col-xl-4" :class="{ 'order-xl-1': (index + 1) % 2 == 0 }">
            <div class="box_item_info" data-jarallax-element="-25">
              <small>Chỉ từ {{ new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(room.cost) }}/đêm</small>
              <h2>{{ room.room_name }}</h2>
              <p>{{ room.description }}</p>
              <div class="facilities clearfix">
                <ul>
                  <li v-for="facility in room.facilities_rooms">
                    <i :class="facility.facility.icon"></i>
                    {{ facility.facility.name }}
                  </li>
                </ul>
              </div>
              <div class="box_item_footer d-flex align-items-center justify-content-between">
                <a href="#0" class="btn_4 learn-more">
                  <span class="circle">
                    <span class="icon arrow"></span>
                  </span>
                  <span @click="book_now(room)" class="button-text">Đặt ngay</span>
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="pinned-image pinned-image--medium">
      <div class="pinned-image__container" id="section_video">
        <video loop="loop" muted="muted" id="video_home">
          <source src="../../../assets/main/video/sunset.mp4" type="video/mp4" />
          <!-- <source src="../../../assets/main/video/sunset.webm" type="video/webm" /> -->
          <!-- <source src="../../../assets/main/video/sunset.ogv" type="video/ogg" /> -->
        </video>
        <div class="pinned-image__container-overlay"></div>
      </div>
      <div class="pinned_over_content">
        <div class="title white center mb-5">
          <small data-cue="slideInUp">Phuong Uyen Homestay</small>
          <h2 data-cue="slideInUp" data-delay="100">Cam kết</h2>
        </div>
        <div class="row mt-4">
          <div class="col-xl-3 col-lg-6 col-md-6 col-6">
            <div class="box_facilities white no-border" data-cue="slideInUp">
              <i class="fa-sharp fa-solid fa-broom mb-5"></i>
              <h3>Sạch sẽ</h3>
            </div>
          </div>
          <div class="col-xl-3 col-lg-6 col-md-6 col-6">
            <div class="box_facilities white" data-cue="slideInUp">
              <i class="fa-sharp fa-solid fa-shield-check mb-5"></i>
              <h3>An toàn</h3>
            </div>
          </div>
          <div class="col-xl-3 col-lg-6 col-md-6 col-6">
            <div class="box_facilities white" data-cue="slideInUp">
              <i class="fa-solid fa-person-running-fast mb-5"></i>
              <h3>Nhanh chóng</h3>
            </div>
          </div>
          <div class="col-xl-3 col-lg-6 col-md-6 col-6">
            <div class="box_facilities white" data-cue="slideInUp">
              <i class="fa-solid fa-hundred-points mb-5"></i>
              <h3>Uy tín</h3>
            </div>
          </div>
        </div>
        <!-- /Row -->
      </div>
    </div>
    <!-- /pinned-content -->
  </main>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import axios from "@/utils/axios";
import { toast } from "vue-sonner";
import router from "@/router";
import DatePicker from "@/components/main/utils/DatePicker.vue";

const route = useRoute();

const branches = ref([]);
const rooms = ref([]);
const branch_selected = ref({ name: "Chọn chi nhánh" });
const date_booking = ref("");
const searching = ref(false);
const adults_booking = ref(2);

const update_search_room = async () => {
  const [startDate, endDate] = date_booking.value.split(" - ");
  if (branch_selected.value.name === "Chọn chi nhánh") toast.warning("Vui lòng chọn chi nhánh !");
  else if (!date_booking.value) toast.warning("Vui lòng chọn ngày đặt phòng !");
  else if (adults_booking.value < 1) toast.warning("Số người lớn phải lớn hơn 0 !");
  else if (!check_date_valid(startDate, endDate)) searching.value = false;
  else {
    try {
      searching.value = true;
      const response = await axios.get("/room/search", {
        params: {
          start_date: startDate,
          end_date: endDate,
          branch_id: branch_selected.value.id,
          room_adults: adults_booking.value,
        },
      });
      if (response.data.success) {
        toast.success("Đã tìm thầy phòng phù hợp với nhu cầu của bạn !");

        window.location.href = router.resolve({
          name: "room-search",
          query: {
            branch_id: branch_selected.value.id,
            start_date: startDate,
            end_date: endDate,
            adults: adults_booking.value,
          },
        }).href;
      } else {
        toast.error(response.data.message);
      }
    } finally {
      searching.value = false;
    }
  }
};

const check_date_valid = (startDate, endDate) => {
  if (startDate == null || endDate == null) {
    toast.error("Ngày đặt phòng không hợp lệ !");
  } else {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (start > end) toast.error("Ngày đặt phòng không hợp lệ !");
    else if (start < today) toast.error("Ngày đặt phòng không hợp lệ !");
    else return true;
  }
};

const valid_query = () => {
  const isValidDate = (dateString) => {
    const regex = /^\d{2}\/\d{2}\/\d{4}$/;
    if (!regex.test(dateString)) {
      return { success: false, message: "Ngày không hợp lệ" };
    }
    const [month, day, year] = dateString.split("/").map(Number);
    const date = new Date(year, month - 1, day);
    return date.getFullYear() === year && date.getMonth() === month - 1 && date.getDate() === day ? { success: true } : { success: false, message: "Ngày không hợp lệ" };
  };

  const { start_date, end_date, adults, branch_id } = route.query;

  if (isNaN(branch_id)) {
    return {
      success: false,
      message: "Chi nhánh không hợp lệ",
    };
  }
  if (!isValidDate(start_date).success || !isValidDate(end_date).success)
    return {
      success: false,
      message: "Ngày không hợp lệ",
    };

  if (isNaN(adults))
    return {
      success: false,
      message: "Số lượng người lớn và trẻ em phải là số",
    };

  return {
    success: true,
    data: {
      start_date,
      end_date,
      adults,
      branch_id,
    },
  };
};

const book_now = (room) => {
  const logged = localStorage.getItem("jwt_token_website");
  if (!logged) {
    toast.error("Vui lòng đăng nhập trước khi đặt phòng !");
    window.location.href = router.resolve({
      name: "main_login",
    }).href;
  } else {
    window.location.href = router.resolve({
      name: "room-booking",
      query: {
        branch_id: room.branch.id,
        room_id: room.id,
        start_date: route.query.start_date,
        end_date: route.query.end_date,
        adults: route.query.adults,
      },
    }).href;
  }
};

const return_home = () => {
  setTimeout(() => {
    window.location.href = router.resolve({
      name: "home",
    }).href;
  }, 2000);
};
// Fetch Data
const fetchBranches = async () => {
  try {
    const response = await axios.get("/branch/get-by-id", {
      params: {
        id: route.query.branch_id,
      },
    });

    if (!response.data.success) {
      toast.error(response.data.message);
      return_home();
    } else {
      try {
        const response = await axios.get("/branch/get-optimal");

        if (response.data.success) {
          branches.value = response.data.data;
          branch_selected.value = branches.value.find((branch) => branch.id == route.query.branch_id);
        } else {
          toast.error(response.data.message);
          return_home();
        }
      } catch (error) {
        console.error(error);
      }
    }
  } catch (error) {
    console.error(error);
  }
};

const fetchRooms = async () => {
  try {
    const response = await axios.get("/room/search", {
      params: {
        start_date: route.query.start_date,
        end_date: route.query.end_date,
        branch_id: route.query.branch_id,
        room_adults: route.query.adults,
      },
    });
    if (response.data.success) {
      rooms.value = response.data.data;
    } else {
      toast.error(response.data.message);
      return_home();
    }
  } catch (error) {
    console.error(error);
  }
};

onMounted(() => {
  date_booking.value = `${route.query.start_date} - ${route.query.end_date}`;
  const result = valid_query();
  if (!result.success) {
    toast.error(result.message);
    return_home();
    return;
  } else {
    adults_booking.value = result.data.adults;
  }
  fetchBranches();
  fetchRooms();
});
</script>
