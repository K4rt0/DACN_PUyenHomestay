<template>
  <div class="card">
    <div class="card-header">
      <div class="d-flex justify-content-between">
        <h4 class="m-0">Thêm chi nhánh</h4>
        <button v-if="!loading" type="button" @click="add_contact" class="btn btn-sm btn-primary">Thêm</button>
        <button v-else type="button" disabled class="btn btn-sm btn-primary">
          <div class="spinner-border" style="font-size: 20px"></div>
        </button>
      </div>
    </div>
    <div class="card-body">
      <div class="row g-6">
        <!-- Branch Name -->
        <div class="col-12">
          <label class="form-label" for="branh_name">Tên chi nhánh</label>
          <input v-model="branch_name" type="text" id="branh_name" class="form-control" placeholder="Chi nhánh 1 ..." />
        </div>

        <!-- Provinces -->
        <div class="col-12 col-md-12 col-lg-12">
          <label class="form-label" for="address">Địa chỉ</label>
          <input v-model="address" type="text" id="address" class="form-control" placeholder="" />
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="province">Thành phố</label>
          <div class="position-relative">
            <select id="province" class="select2 form-select select2-hidden-accessible" tabindex="-1" @change="onProvinceChange" v-model="selectedProvince">
              <option value="" selected>Chọn Thành phố</option>
              <option v-for="province in provinces" :key="province.id" :value="province.id">{{ province.full_name }}</option>
            </select>
          </div>
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="district">Quận / Huyện</label>
          <div class="position-relative">
            <select id="district" class="select2 form-select select2-hidden-accessible" tabindex="-1" @change="onDistrictChange" v-model="selectedDistrict">
              <option value="" selected>Chọn Quận / Huyện</option>
              <option v-for="district in districts" :key="district.id" :value="district.id">{{ district.full_name }}</option>
            </select>
          </div>
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="ward">Phường / Xã</label>
          <div class="position-relative">
            <select id="ward" class="select2 form-select select2-hidden-accessible" tabindex="-1" v-model="selectedWard">
              <option value="" selected>Chọn Phường / Xã</option>
              <option v-for="ward in wards" :key="ward.id" :value="ward.id">{{ ward.full_name }}</option>
            </select>
          </div>
        </div>

        <div class="col-12">
          <div v-if="branch_contact_selected.length != 0">
            <label class="form-label" for="description">Các liên hệ đã thêm</label>
            <ol class="list-group list-group-numbered mb-5">
              <li v-for="contact in branch_contact_selected" :key="contact.id" class="list-group-item d-flex align-items-center px-3 justify-content-around">
                <span class="col-11">
                  <i :class="[contact.contact_icon]" class="d-inline-block fs-5 me-1 ms-2"></i> {{ contact.name }}: <b class="me-5">{{ contact.value }}</b>
                </span>
                <button @click="edit_contact(contact)" class="btn btn-sm p-1 fs-5">
                  <i class="fa-duotone fa-solid fa-pen-to-square"></i>
                </button>
                <button data-bs-toggle="modal" data-bs-target="#modal-delete" @click="contact_delete = contact" class="btn btn-sm p-1 ms-3 fs-5"><i class="fa-solid fa-trash"></i></button>
              </li>
            </ol>
          </div>
          <div class="row">
            <div class="col-3">
              <label class="form-label" for="contact">Liên hệ</label>
              <div class="d-flex align-items-end">
                <i v-if="contact_selected != 0" :class="[get_icon_class(contact_selected), 'fs-2', 'me-3']"></i>
                <div class="w-100">
                  <select id="contact" class="form-select" v-model="contact_selected">
                    <option value="0" selected>Chọn liên hệ</option>
                    <option v-for="(contact, index) in branch_contact" :value="contact.id" :key="contact.id">{{ index + 1 }}. {{ contact.name }}</option>
                  </select>
                </div>
              </div>
            </div>
            <div class="col-8">
              <label class="form-label" for="content">Nội dung</label>
              <input id="content" v-model="contact_value" type="text" class="form-control" />
            </div>
            <div class="col-1 align-content-end text-end">
              <button @click="save_contact" type="button" class="btn btn-primary">Lưu</button>
            </div>
          </div>
        </div>

        <!-- Description -->
        <div class="col-12">
          <label class="form-label" for="description">Mô tả</label>
          <textarea v-model="branch_desc" name="description" class="form-control" id="description" rows="5"></textarea>
        </div>

        <div class="modal fade" id="modal-delete" data-bs-backdrop="static" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="modalCenterTitle">Xoá liên hệ</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" @click="close_modal_delete" aria-label="Close"></button>
              </div>
              <div class="modal-body">Bạn có chắc chắn muốn xóa liên hệ này không?</div>
              <div class="modal-footer">
                <button type="button" class="btn btn-label-secondary" @click="close_modal_delete" data-bs-dismiss="modal">Huỷ</button>
                <button type="button" class="btn btn-primary" @click="confirm_delete" data-bs-dismiss="modal">Xoá</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import axios from "@/utils/axios";
import { ref, onMounted } from "vue";
import { toast } from "vue-sonner";
import router from "@/router";
import breadcrumb from "@/utils/breadcrumb";

breadcrumb([
  { path: "admin_branch_list", name: "Branch List" },
  { path: "admin_dashboard", name: "New" },
]);

const branch_name = ref("");
const branch_desc = ref("");
const branch_contact = ref([]);
const branch_contact_selected = ref([]);

const contact_selected = ref(0);
const contact_value = ref("");
const contact_editing = ref(null);
const contact_delete = ref(null);
const loading = ref(false);

const add_contact = async () => {
  if (branch_name.value === "") toast.warning("Vui lòng nhập tên chi nhánh !");
  else if (selectedProvince.value === "") toast.warning("Vui lòng chọn Thành phố !");
  else if (selectedDistrict.value === "") toast.warning("Vui lòng chọn Quận / Huyện !");
  else if (selectedWard.value === "") toast.warning("Vui lòng chọn Phường / Xã !");
  else if (branch_contact_selected.value.length === 0) toast.warning("Vui lòng thêm thông tin liên hệ !");
  else {
    loading.value = true;
    const branch_contact_details = branch_contact_selected.value.map((contact) => {
      return {
        value: contact.value,
        branch_contact: {
          id: contact.contact_id,
        },
      };
    });
    const branch = {
      name: branch_name.value,
      address: address.value,
      province: provinces.value.find((province) => province.id == selectedProvince.value).full_name,
      district: districts.value.find((district) => district.id == selectedDistrict.value).full_name,
      ward: wards.value.find((ward) => ward.id == selectedWard.value).full_name,
      description: branch_desc.value,
      branch_contact_details: branch_contact_details,
    };
    try {
      const response = await axios.post("/branch", branch, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });
      if (response.data.success === true) {
        router.push({ name: "admin_branch_list" });
        toast.success("Tạo mới chi nhánh thành công !");
      } else {
        loading.value = false;
        toast.error("Đã có lỗi xảy ra khi tạo mới chi nhánh !");
      }
    } catch (error) {
      loading.value = false;
    }
  }
};

function fetchBranchContact() {
  axios.get("/branch-contact").then((response) => {
    branch_contact.value = response.data.data;
  });
}

function get_icon_class(contact_id) {
  const contact = branch_contact.value.find((contact) => contact.id == contact_id);
  if (!contact) return "";
  return contact.contact_icon;
}

function get_icon_name(contact_id) {
  const contact = branch_contact.value.find((contact) => contact.id == contact_id);
  if (!contact) return "";
  return contact.name;
}

function save_contact() {
  if (contact_selected.value == 0 || contact_value.value === "") toast.warning("Thông tin liên hệ không hợp lệ !");
  else {
    if (contact_editing.value == null) {
      let length = branch_contact_selected.value.length;
      branch_contact_selected.value.push({
        id: length == 0 ? 0 : branch_contact_selected.value[length - 1].id + 1,
        name: get_icon_name(contact_selected.value),
        contact_id: contact_selected.value,
        contact_icon: get_icon_class(contact_selected.value),
        value: contact_value.value,
      });

      contact_selected.value = 0;
      contact_value.value = "";
    } else {
      const contactIndex = branch_contact_selected.value.findIndex((item) => item.id === contact_editing.value.id);
      if (contactIndex !== -1) {
        branch_contact_selected.value[contactIndex].name = get_icon_name(contact_selected.value);
        branch_contact_selected.value[contactIndex].contact_id = contact_selected.value;
        branch_contact_selected.value[contactIndex].contact_icon = get_icon_class(contact_selected.value);
        branch_contact_selected.value[contactIndex].value = contact_value.value;

        contact_selected.value = 0;
        contact_value.value = "";
        contact_editing.value = null;
      } else toast.warning("Đã có lỗi xảy ra khi xử lý dữ liệu. Vui lòng reload lại trang website để tiến hành tạo mới chi nhánh !");
    }
  }
}

function close_modal_delete() {
  if (contact_delete.value != null) {
    contact_delete.value = null;
  }
}

function confirm_delete() {
  if (contact_delete.value != null) {
    const contactIndex = branch_contact_selected.value.findIndex((item) => item.id === contact_delete.value.id);
    if (contactIndex !== -1) {
      branch_contact_selected.value.splice(contactIndex, 1);
      contact_delete.value = null;
    }
  }
}

function edit_contact(contact) {
  contact_editing.value = contact;
  contact_selected.value = contact.contact_id;
  contact_value.value = contact.value;
}

// Provinces
const provinces = ref([]);
const districts = ref([]);
const wards = ref([]);
const address = ref("");
const selectedProvince = ref("");
const selectedDistrict = ref("");
const selectedWard = ref("");

function fetchProvinces() {
  axios.get("/vietnam/provinces").then((response) => {
    provinces.value = response.data.data;
  });
}

function onProvinceChange() {
  if (selectedProvince.value) {
    axios
      .get(`/vietnam/districts/${selectedProvince.value}`)
      .then((response) => {
        districts.value = response.data.data;
        wards.value = []; // Clear wards when province changes
        selectedDistrict.value = "";
        selectedWard.value = "";
      })
      .catch((error) => {
        console.log(error);
      });
  }
}

function onDistrictChange() {
  if (selectedDistrict.value) {
    axios
      .get(`/vietnam/wards/${selectedDistrict.value}`)
      .then((response) => {
        wards.value = response.data.data;
        selectedWard.value = ""; // Clear ward selection when district changes
      })
      .catch((error) => {
        console.log(error);
      });
  }
}

onMounted(() => {
  window.addEventListener("keydown", close_modal_delete);
  fetchProvinces();
  fetchBranchContact();
});
</script>
