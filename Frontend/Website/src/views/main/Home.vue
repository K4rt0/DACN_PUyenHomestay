<template>
  <main>
    <div class="hero home-search full-height jarallax">
      <video class="jarallax-img position-absolute" loop muted autoplay>
        <source src="../../assets/main/video/sunset.mp4" type="video/mp4" />
      </video>
      <div class="wrapper opacity-mask d-flex align-items-center justify-content-center text-center animate_hero is-transitioned" data-opacity-mask="rgba(0, 0, 0, 0.5)">
        <div class="container">
          <small class="slide-animated one">Trải nghiệm Homestay cao cấp</small>
          <h3 class="slide-animated two">Một nơi ở độc đáo<br />dành cho hộ gia đình</h3>
          <div class="row g-0 booking_form">
            <div class="col-lg-4">
              <div class="form-group">
                <DatePicker class="form-control" v-model="date_booking" @update:date="updateDate" placeholder="Check in / Check out" readonly />
                <i class="bi bi-calendar2"></i>
              </div>
            </div>
            <div class="col-lg-3">
              <div class="nice-select wide mb-0 border-0 border-end" tabindex="0">
                <span class="current">Chọn chi nhánh</span>
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
              <button v-else type="button" @click="search_room" class="text-center">Tìm kiếm</button>
            </div>
          </div>
        </div>
        <div class="mouse_wp slide-animated four">
          <a href="#first_section" class="btn_scrollto">
            <div class="mouse"></div>
          </a>
        </div>
        <!-- /mouse_wp -->
      </div>
    </div>
    <!-- /jarallax video background -->

    <div class="pattern_2">
      <div class="container margin_120_95" id="first_section">
        <div class="row justify-content-between flex-lg-row-reverse align-items-center">
          <div class="col-lg-5">
            <div class="parallax_wrapper">
              <img src="../../assets/main/img/home_2.jpg" alt="" class="img-fluid rounded-img" />
              <div data-cue="slideInUp" class="img_over">
                <span data-jarallax-element="-30"><img src="../../assets/main/img/home_1.jpg" alt="" class="rounded-img" /></span>
              </div>
            </div>
          </div>
          <div class="col-lg-5">
            <div class="intro">
              <div class="title">
                <small>About us</small>
                <h2>Tailored services and the experience of unique holidays</h2>
              </div>
              <p class="lead">Vivamus volutpat eros pulvinar velit laoreet, sit amet egestas erat dignissim.</p>
              <p>Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.</p>
              <p><em>Maria...the Owner</em></p>
            </div>
          </div>
        </div>
        <!-- /Row -->
      </div>
      <div class="pinned-image pinned-image--medium">
        <div class="pinned-image__container" id="section_video">
          <video loop muted id="video_home">
            <source src="../../assets/main/video/swimming_pool_2.mp4" type="video/mp4" />
            <!-- <source src="../../assets/main/video/swimming_pool_2.webm" type="video/webm" /> -->
            <!-- <source src="../../assets/main/video/swimming_pool_2.ogv" type="video/ogg" /> -->
          </video>
          <div class="pinned-image__container-overlay"></div>
        </div>
        <div class="pinned_over_content">
          <div class="title white">
            <small data-cue="slideInUp" data-delay="200">Luxury Hotel Experience</small>
            <h2 data-cue="slideInUp" data-delay="300">
              Enjoy in a very<br />
              Immersive Relax
            </h2>
          </div>
        </div>
      </div>
      <!-- /pinned content -->
    </div>
    <!-- /Pattern  -->
  </main>
</template>

<script setup>
import router from "@/router";
import axios from "@/utils/axios";
import { onMounted, ref } from "vue";
import { toast } from "vue-sonner";
import DatePicker from "@/components/main/utils/DatePicker.vue";

const branches = ref([]);
const branch_selected = ref({ name: "Chọn chi nhánh" });
const date_booking = ref("");
const adults_booking = ref(2);
const searching = ref(false);

const search_room = async () => {
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

// Fetch Data
const fetchBranches = async () => {
  try {
    const response = await axios.get("/branch/get-optimal");

    if (response.data.success) {
      branches.value = response.data.data;
    } else toast.error(response.data.message);
  } catch (error) {
    console.error(error);
  }
};

const updateDate = (newDate) => {
  date_booking.value = newDate;
};

onMounted(() => {
  fetchBranches();
});
</script>
