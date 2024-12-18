<template>
  <div class="d-flex justify-content-between mb-5">
    <h4 class="m-0">Thêm phòng</h4>
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
          <select id="branch" class="form-select" v-model="branch_selected">
            <option value="0" selected>Chọn liên hệ</option>
            <option v-for="(contact, index) in branches" :value="contact.id" :key="contact.id">{{ index + 1 }}. {{ contact.name }}</option>
          </select>
        </div>
        <div class="col-12 col-md-3 col-lg-4 mb-lg-3">
          <label class="form-label" for="room_num">Số phòng</label>
          <input v-model="room_number" type="text" id="room_num" class="form-control" placeholder="01.01" />
        </div>
        <div class="col-12 col-md-3 col-lg-4 mb-lg-3">
          <label class="form-label" for="room_name">Tên phòng</label>
          <input v-model="room_name" type="text" id="room_name" class="form-control" placeholder="Phòng đơn, phòng đôi,..." />
        </div>
        <div class="col-12 col-md-3 col-lg-4">
          <label class="form-label" for="room_cost">Giá tiền</label>
          <input v-model="room_cost" type="number" min="1" id="room_cost" class="form-control" placeholder="1,000,000đ" />
        </div>
        <div class="col-12 col-md-3 col-lg-4">
          <label class="form-label" for="room_max_adults">Số lượng khách tối đa</label>
          <input v-model="room_max_adults" type="number" min="1" id="room_max_adults" class="form-control" placeholder="2" />
        </div>
        <div class="col-12 col-md-3 col-lg-4">
          <label class="form-label" for="room_width">Kích thước</label>
          <input v-model="room_width" type="number" min="1" id="room_width" class="form-control" placeholder="10" />
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
  <div class="card mb-5">
    <div class="card-header">
      <h4 class="mb-0">Hình mô tả</h4>
    </div>
    <div class="card-body">
      <div class="row">
        <div v-for="(image, index) in previewImages" :key="index" class="col-2 position-relative">
          <div class="border border-3 border-primary rounded w-100 position-relative" style="padding-top: 100%">
            <img :src="image.preview" class="rounded object-fit-cover position-absolute w-100 h-100 top-0 left-0" />
            <button @click="removeImage(image)" class="btn-close position-absolute top-0 end-0 m-1" style="z-index: 1"></button>
          </div>
        </div>
      </div>
      <input ref="fileInput" class="form-control mt-5" type="file" accept="image/*" @change="handleFileUpload" multiple />
    </div>
  </div>
</template>
<script setup>
import axios from "@/utils/axios";
import breadcrumb from "@/utils/breadcrumb";
import { onMounted, ref } from "vue";
import { toast } from "vue-sonner";

breadcrumb([
  { path: "admin_room_list", name: "Danh sách phòng ngủ" },
  { path: "admin_room_new", name: "Thêm phòng ngủ" },
]);

const branches = ref([]);
const branch_selected = ref(0);
const facilities = ref([]);
const facilities_selected = ref([]);

const room_number = ref(null);
const room_width = ref(null);
const room_name = ref(null);
const room_max_adults = ref(null);
const room_cost = ref(null);
const loading = ref(false);

const previewImages = ref([]);
const filesArray = ref([]);
const fileInput = ref(null);

const save_room = async () => {
  if (room_number.value === null) toast.warning("Vui lòng nhập số phòng !");
  else if (room_width.value === null) toast.warning("Vui lòng nhập kích thước phòng !");
  else if (room_cost.value === null) toast.warning("Vui lòng nhập giá phòng !");
  else if (branch_selected.value === 0) toast.warning("Vui lòng chọn chi nhánh !");
  else if (facilities_selected.value.length == 0) toast.warning("Vui lòng chọn ít nhất 1 tiện nghi !");
  else if (filesArray.value.length == 0) toast.warning("Vui lòng chọn ít nhất 1 hình ảnh !");
  else {
    loading.value = true;

    const roomData = {
      room_number: room_number.value,
      room_name: room_name.value,
      cost: parseFloat(room_cost.value),
      room_width: parseInt(room_width.value),
      room_width: parseInt(room_width.value),
      room_max_adults: parseInt(room_max_adults.value),
      branch: { id: branch_selected.value },
      facilities_rooms: facilities_selected.value.map((id) => ({
        facility: { id: id },
        is_bed: false,
        quantity: 1,
      })),
    };

    const formData = new FormData();
    formData.append("roomJson", JSON.stringify(roomData));

    filesArray.value.forEach((file) => {
      formData.append(`images`, file);
    });

    try {
      const response = await axios.post("/room", formData, {
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

const removeImage = (image) => {
  previewImages.value = previewImages.value.filter((img) => img !== image);
  filesArray.value = filesArray.value.filter((file) => file !== image.file);

  updateFileInput();
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

onMounted(() => {
  fetchBranches();
  fetchFacilities();
});
</script>
