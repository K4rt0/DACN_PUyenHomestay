<template>
  <div v-if="loading_data" class="d-flex flex-column align-items-center" style="padding: 100px 0">
    <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
    <h3 class="m-0">Đang tải dữ liệu...</h3>
  </div>
  <div v-else>
    <div class="d-flex justify-content-between mb-5">
      <h4 class="m-0">Sửa phòng</h4>
      <div>
        <router-link :to="{ name: 'admin_room_list' }" class="btn btn-secondary me-1">Trở lại</router-link>
        <button v-if="!loading" type="button" @click="save_room" class="btn btn-primary">Thêm</button>
        <button v-else type="button" disabled class="btn btn-primary">
          <div class="spinner-border" style="font-size: 20px"></div>
        </button>
      </div>
    </div>
    <div class="card mb-5">
      <div class="card-header">
        <h4 class="mb-0">Thông tin phòng</h4>
      </div>
      <div class="card-body">
        <div class="row">
          <div class="col-12 col-md-3 col-lg-4 mb-lg-3">
            <label class="form-label" for="branch">Chi nhánh</label>
            <select id="branch" class="form-select" v-model="room.branch.id">
              <option value="0" selected>Chọn liên hệ</option>
              <option v-for="(contact, index) in branches" :value="contact.id" :key="contact.id">{{ index + 1 }}. {{ contact.name }}</option>
            </select>
          </div>
          <div class="col-12 col-md-3 col-lg-4 mb-lg-3">
            <label class="form-label" for="room_num">Số phòng</label>
            <input v-model="room.room_number" type="text" id="room_num" class="form-control" placeholder="01.01" />
          </div>
          <div class="col-12 col-md-3 col-lg-4 mb-lg-3">
            <label class="form-label" for="room_name">Tên phòng</label>
            <input v-model="room.room_name" type="text" id="room_name" class="form-control" placeholder="Phòng đơn, phòng đôi,..." />
          </div>
          <div class="col-12 col-md-3 col-lg-4">
            <label class="form-label" for="room_cost">Giá tiền</label>
            <input v-model="room.cost" type="number" min="1" id="room_cost" class="form-control" placeholder="1,000,000đ" />
          </div>
          <div class="col-12 col-md-3 col-lg-4">
            <label class="form-label" for="room_max_adults">Số lượng khách tối đa</label>
            <input v-model="room.room_max_adults" type="number" min="1" id="room_max_adults" class="form-control" placeholder="2" />
          </div>
          <div class="col-12 col-md-3 col-lg-4">
            <label class="form-label" for="room_width">Kích thước</label>
            <input v-model="room.room_width" type="number" min="1" id="room_width" class="form-control" placeholder="10" />
          </div>
        </div>
      </div>
    </div>
    <div class="card mb-5">
      <div class="card-header">
        <h4 class="mb-0">Tiện nghi</h4>
      </div>
      <div class="card-body">
        <div class="row">
          <div v-for="facility in facilities" :key="facility.id" class="col-12 col-md-2 col-lg-2">
            <div class="form-check">
              <input class="form-check-input" type="checkbox" :id="'facility_' + facility.id" :value="facility.id" v-model="facilities_selected" />
              <label class="form-check-label" :for="'facility_' + facility.id"><i :class="[facility.icon]" class="mx-1"></i> {{ facility.name }}</label>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div v-if="room.room_images.length" class="card mb-5">
      <div class="card-header">
        <h4 class="mb-0">Hình mô tả</h4>
      </div>
      <div class="card-body">
        <div class="row">
          <div v-for="(image, index) in room.room_images" :key="index" class="col-2 position-relative">
            <div class="border border-3 border-primary rounded w-100 position-relative" style="padding-top: 100%">
              <img :src="'https://i.imgur.com/' + image.url + '.png'" class="rounded object-fit-cover position-absolute w-100 h-100 top-0 left-0" />
              <button @click="removeImage(image, false)" class="btn-close position-absolute top-0 end-0 m-1" style="z-index: 1"></button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="card mb-5">
      <div class="card-header">
        <h4 class="mb-0">Thêm hình mô tả</h4>
      </div>
      <div class="card-body">
        <div class="row">
          <div v-for="(image, index) in previewImages" :key="index" class="col-2 position-relative">
            <div class="border border-3 border-primary rounded w-100 position-relative" style="padding-top: 100%">
              <img :src="image.preview" class="rounded object-fit-cover position-absolute w-100 h-100 top-0 left-0" />
              <button @click="removeImage(image, true)" class="btn-close position-absolute top-0 end-0 m-1" style="z-index: 1"></button>
            </div>
          </div>
        </div>
        <input ref="fileInput" class="form-control mt-5" type="file" accept="image/*" @change="handleFileUpload" multiple />
      </div>
    </div>
  </div>
</template>
<script setup>
import axios from "@/utils/axios";
import breadcrumb from "@/utils/breadcrumb";
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";

const route = useRoute();

breadcrumb([
  { path: "admin_room_list", name: "Danh sách phòng ngủ" },
  { path: "admin_room_new", name: route.query.name },
]);

const branches = ref([]);
const facilities = ref([]);
const facilities_selected = ref([]);

const room = ref(null);
const loading = ref(false);
const loading_data = ref(true);

const previewImages = ref([]);
const filesArray = ref([]);
const fileInput = ref(null);

const save_room = async () => {
  if (room.value.room_number.value === null) toast.warning("Vui lòng nhập số phòng !");
  else if (room.value.room_width.value === null) toast.warning("Vui lòng nhập kích thước phòng !");
  else if (room.value.room_name.value === null || room.value.room_name.value === "") toast.warning("Vui lòng nhập kích thước phòng !");
  else if (room.value.room_max_adults.value === null || room.value.room_max_adults.value === "") toast.warning("Vui lòng nhập kích thước phòng !");
  else if (room.value.cost.value === null) toast.warning("Vui lòng nhập giá phòng !");
  else if (room.value.branch.id === 0) toast.warning("Vui lòng chọn chi nhánh !");
  else if (facilities_selected.value.length == 0) toast.warning("Vui lòng chọn ít nhất 1 tiện nghi !");
  else if (filesArray.value.length == 0 && room.value.room_images.length == 0) toast.warning("Vui lòng chọn ít nhất 1 hình ảnh !");
  else {
    loading.value = true;

    const roomData = {
      room_number: room.value.room_number,
      room_name: room.value.room_name,
      cost: parseFloat(room.value.cost),
      room_width: parseInt(room.value.room_width),
      room_max_adults: parseInt(room.value.room_max_adults),
      branch: { id: room.value.branch.id },
      facilities_rooms: facilities_selected.value.map((id) => ({
        facility: { id: id },
        is_bed: false,
        quantity: 1,
      })),
      room_images: room.value.room_images,
    };

    console.log(roomData);

    const formData = new FormData();
    formData.append("roomJson", JSON.stringify(roomData));

    filesArray.value.forEach((file) => {
      formData.append(`images`, file);
    });

    try {
      const response = await axios.put(`/room/${route.query.id}`, formData, {
        headers: {
          "Content-Type": "multipart/form-data",
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });

      if (response.data.success) {
        toast.success(response.data.message);
        setTimeout(() => {
          window.location.href = "/admin/room-list";
        }, 1000);
      } else toast.error(response.data.message);
    } finally {
      loading.value = false;
    }
  }
};
const handleFileUpload = (event) => {
  const newFiles = Array.from(event.target.files);

  newFiles.forEach((file) => {
    filesArray.value.push(file);
    const reader = new FileReader();
    reader.onload = (e) => {
      previewImages.value.push({ file: file, preview: e.target.result });
    };
    reader.readAsDataURL(file);
  });

  updateFileInput();
};

const removeImage = (image, is_new) => {
  if (is_new) {
    previewImages.value = previewImages.value.filter((img) => img !== image);
    filesArray.value = filesArray.value.filter((file) => file !== image.file);
    updateFileInput();
  } else {
    room.value.room_images = room.value.room_images.filter((img) => img !== image);
  }
};

const updateFileInput = () => {
  const dataTransfer = new DataTransfer();
  filesArray.value.forEach((file) => dataTransfer.items.add(file));
  if (fileInput.value) {
    fileInput.value.files = dataTransfer.files;
  }
};

// Fetch Data
const fetchBranches = async () => {
  try {
    const response = await axios.get("/branch");

    if (response.data.success) {
      branches.value = response.data.data;
    } else toast.error(response.data.message);
  } finally {
  }
};

const fetchFacilities = async () => {
  try {
    const response = await axios.get("/facilities");

    if (response.data.success) {
      facilities.value = response.data.data;
    } else toast.error(response.data.message);
  } finally {
  }
};

const fetchRoom = async () => {
  try {
    const response = await axios.get(`/room/${route.query.id}`);

    if (response.data.success) {
      room.value = response.data.data;
      console.log(room.value);
      room.value.facilities_rooms.forEach((facility) => {
        facilities_selected.value.push(facility.facility.id);
      });
    } else toast.error(response.data.message);
  } finally {
    loading_data.value = false;
  }
};

onMounted(() => {
  fetchRoom();
  fetchBranches();
  fetchFacilities();
});
</script>
