<template>
  <div class="card">
    <div class="card-header">
      <div class="d-flex justify-content-between">
        <h4 class="m-0">Sửa chi nhánh</h4>
        <span>
          <router-link :to="{ name: 'admin_branch_list' }" type="button" class="btn btn-sm btn-secondary me-2">Trở về</router-link>
          <button v-if="!loading" type="button" @click="save_branch" class="btn btn-sm btn-primary">Lưu</button>
          <button v-else type="button" disabled class="btn btn-sm btn-primary">
            <div class="spinner-border" style="font-size: 20px"></div>
          </button>
        </span>
      </div>
    </div>
    <div v-if="loading_data" class="d-flex flex-column align-items-center" style="padding: 100px 0">
      <div class="spinner-border border-5 mb-3" style="width: 50px; height: 50px"></div>
      <h3 class="m-0">Đang tải dữ liệu...</h3>
    </div>
    <div v-else class="card-body">
      <div class="row g-6">
        <!-- Branch Name -->
        <div class="col-12">
          <label class="form-label" for="branh_name">Tên chi nhánh</label>
          <input v-model="branch.name" type="text" id="branh_name" class="form-control" placeholder="Chi nhánh 1 ..." />
        </div>

        <!-- Provinces -->
        <div class="col-12 col-md-12 col-lg-12">
          <label class="form-label" for="address">Địa chỉ</label>
          <input v-model="branch.address" type="text" id="address" class="form-control" placeholder="" />
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="province">Thành phố</label>
          <div class="position-relative">
            <select id="province" class="select2 form-select select2-hidden-accessible" tabindex="-1" @change="onProvinceChange" v-model="branch.province">
              <option value="0">Chọn Thành phố</option>
              <option v-for="province in provinces" :key="province.id" :value="province.full_name">{{ province.full_name }}</option>
            </select>
          </div>
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="district">Quận / Huyện</label>
          <div class="position-relative">
            <select id="district" class="select2 form-select select2-hidden-accessible" tabindex="-1" @change="onDistrictChange" v-model="branch.district">
              <option value="0" selected>Chọn Quận / Huyện</option>
              <option v-for="district in districts" :key="district.id" :value="district.full_name">{{ district.full_name }}</option>
            </select>
          </div>
        </div>
        <div class="col-12 col-md-4 col-lg-4">
          <label class="form-label" for="ward">Phường / Xã</label>
          <div class="position-relative">
            <select id="ward" class="select2 form-select select2-hidden-accessible" tabindex="-1" v-model="branch.ward">
              <option value="0" selected>Chọn Phường / Xã</option>
              <option v-for="ward in wards" :key="ward.id" :value="ward.full_name">{{ ward.full_name }}</option>
            </select>
          </div>
        </div>

        <div class="col-12">
          <label v-if="contact_old.length != 0 || contact_new.length != 0" class="form-label" for="description">Các liên hệ đã thêm</label>
          <div v-if="contact_old.length != 0 || contact_new.length != 0" class="form-control mb-5 py-5">
            <ol v-if="contact_old.length != 0" class="list-group list-group-numbered" :class="{ 'mb-5': contact_new.length != 0 }">
              <label class="form-label" for="description">Liên hệ có sẵn</label>
              <li v-for="contact in contact_old" :key="contact.id" class="list-group-item d-flex align-items-center px-3 justify-content-around">
                <span class="col-11">
                  <i :class="[contact.contact_icon]" class="d-inline-block fs-5 me-1 ms-2"></i> {{ contact.name }}: <b class="me-5">{{ contact.value }}</b>
                </span>
                <button @click="edit_contact(contact, false)" class="btn btn-sm p-1 fs-5">
                  <i class="fa-duotone fa-solid fa-pen-to-square"></i>
                </button>
                <button
                  data-bs-toggle="modal"
                  data-bs-target="#modal-delete"
                  @click="
                    contact_delete = contact;
                    is_new = false;
                  "
                  class="btn btn-sm p-1 ms-3 fs-5"
                >
                  <i class="fa-solid fa-trash"></i>
                </button>
              </li>
            </ol>
            <ol v-if="contact_new.length != 0" class="list-group list-group-numbered">
              <label class="form-label" for="description">Liên hệ mới</label>
              <li v-for="contact in contact_new" :key="contact.id" class="list-group-item d-flex align-items-center px-3 justify-content-around">
                <span class="col-11">
                  <i :class="[contact.contact_icon]" class="d-inline-block fs-5 me-1 ms-2"></i> {{ contact.name }}: <b class="me-5">{{ contact.value }}</b>
                </span>
                <button @click="edit_contact(contact, true)" class="btn btn-sm p-1 fs-5">
                  <i class="fa-duotone fa-solid fa-pen-to-square"></i>
                </button>
                <button
                  data-bs-toggle="modal"
                  data-bs-target="#modal-delete"
                  @click="
                    contact_delete = contact;
                    is_new = true;
                  "
                  class="btn btn-sm p-1 ms-3 fs-5"
                >
                  <i class="fa-solid fa-trash"></i>
                </button>
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
          <textarea v-model="branch.description" name="description" class="form-control" id="description" rows="5"></textarea>
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
import breadcrumb from "@/utils/breadcrumb";
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { toast } from "vue-sonner";
import router from "@/router";

const route = useRoute();
const branch = ref(null);

breadcrumb([
  { path: "admin_branch_list", name: "Branch List" },
  { path: "admin_branch_update", name: route.query.name },
]);

const branch_contact = ref([]);
const contact_old = ref([]);
const contact_new = ref([]);
const is_new = ref(true);
const contact_selecting = ref(null);

const contact_selected = ref(0);

const contact_value = ref("");
const contact_delete = ref(null);

const loading = ref(false);
const loading_data = ref(true);

const save_branch = async () => {
  if (branch.value.name === "") toast.warning("Vui lòng nhập tên chi nhánh !");
  else if (branch.value.province === "") toast.warning("Vui lòng chọn Thành phố !");
  else if (branch.value.district === "") toast.warning("Vui lòng chọn Quận / Huyện !");
  else if (branch.value.ward === "") toast.warning("Vui lòng chọn Phường / Xã !");
  else if (contact_new.value.length === 0 && contact_old.value.length === 0) toast.warning("Vui lòng thêm thông tin liên hệ !");
  else {
    loading.value = true;
    let branch_contact_details = [];
    branch_contact_details = [
      ...contact_new.value.map((contact) => {
        return {
          id: 0,
          value: contact.value,
          branch_contact: {
            id: contact.contact_id,
          },
        };
      }),
      ...contact_old.value.map((contact) => {
        return {
          id: contact.id,
          value: contact.value,
          branch_contact: {
            id: contact.contact_id,
          },
        };
      }),
    ];
    const branch_save = {
      name: branch.value.name,
      address: branch.value.address,
      province: branch.value.province,
      district: branch.value.district,
      ward: branch.value.ward,
      description: branch.value.description,
      branch_contact_details: branch_contact_details,
    };

    try {
      const response = await axios.put(`/branch/${route.query.id}`, branch_save, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
        },
      });
      if (response.data.success === true) {
        router.push({ name: "admin_branch_list" });
        toast.success(response.data.message);
      } else {
        toast.error(response.data.message);
      }
    } finally {
      loading.value = false;
    }
  }
};

const fetchBranchContact = async () => {
  try {
    const response = await axios.get("/branch-contact");
    if (response.data.success) branch_contact.value = response.data.data;
    else toast.error(response.data.message);
  } catch (error) {}
};

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
    if (contact_selecting.value == null) {
      let length = contact_new.value.length;
      contact_new.value.push({
        id: length == 0 ? 0 : contact_new.value[length - 1].id + 1,
        name: get_icon_name(contact_selected.value),
        contact_id: contact_selected.value,
        contact_icon: get_icon_class(contact_selected.value),
        value: contact_value.value,
      });
    } else {
      let contactIndex = null;
      if (is_new.value) contactIndex = contact_new.value.findIndex((item) => item.id === contact_selecting.value.id);
      else contactIndex = contact_old.value.findIndex((item) => item.id === contact_selecting.value.id);

      if (contactIndex == -1) toast.warning("Đã có lỗi xảy ra khi xử lý dữ liệu. Vui lòng tải lại trang và thử lại !");
      else {
        if (is_new.value) {
          contact_new.value[contactIndex].name = get_icon_name(contact_selected.value);
          contact_new.value[contactIndex].contact_id = contact_selected.value;
          contact_new.value[contactIndex].contact_icon = get_icon_class(contact_selected.value);
          contact_new.value[contactIndex].value = contact_value.value;
        } else {
          contact_old.value[contactIndex].name = get_icon_name(contact_selected.value);
          contact_old.value[contactIndex].contact_id = contact_selected.value;
          contact_old.value[contactIndex].contact_icon = get_icon_class(contact_selected.value);
          contact_old.value[contactIndex].value = contact_value.value;
        }
        toast.success("Lưu thông tin liên hệ thành công !");
      }
      contact_selecting.value = null;
    }
    contact_selected.value = 0;
    contact_value.value = "";
  }
}

function close_modal_delete() {
  if (contact_delete.value != null) {
    contact_delete.value = null;
  }
}

function confirm_delete() {
  if (contact_delete.value != null) {
    let contactIndex = null;
    if (is_new.value) contactIndex = contact_new.value.findIndex((item) => item.id === contact_delete.value.id);
    else contactIndex = contact_old.value.findIndex((item) => item.id === contact_delete.value.id);

    if (contactIndex !== -1) {
      if (is_new.value) contact_new.value.splice(contactIndex, 1);
      else contact_old.value.splice(contactIndex, 1);

      contact_delete.value = null;
    } else toast.warning("Đã có lỗi xảy ra khi xử lý dữ liệu. Vui lòng tải lại trang và thử lại !");
  }
}

function edit_contact(contact, isNew) {
  is_new.value = isNew;
  contact_selecting.value = contact;
  contact_selected.value = contact.contact_id;
  contact_value.value = contact.value;
}

// Provinces
const provinces = ref([]);
const districts = ref([]);
const wards = ref([]);

const fetchProvinces = async () => {
  try {
    const response = await axios.get("/vietnam/provinces");
    if (response.data.success) provinces.value = response.data.data;
    else toast.error(response.data.message);
  } catch (error) {
    console.log(error);
  }
};

function get_province_id(province_name) {
  return provinces.value.find((province) => province.full_name === province_name).id;
}

const fetchDistricts = async () => {
  try {
    const response = await axios.get(`/vietnam/districts/${get_province_id(branch.value.province)}`);
    if (response.data.success) {
      districts.value = response.data.data;
      fetchWards();
    } else toast.error(response.data.message);
  } catch (error) {
    console.log(error);
  }
};

function get_district_id(district_name) {
  return districts.value.find((district) => district.full_name === district_name).id;
}

const fetchWards = async () => {
  try {
    const response = await axios.get(`/vietnam/wards/${get_district_id(branch.value.district)}`);
    if (response.data.success) {
      wards.value = response.data.data;
    } else toast.error(response.data.message);
  } catch (error) {
    console.log(error);
  }
};

function onProvinceChange() {
  if (branch.value.province) {
    axios
      .get(`/vietnam/districts/${get_province_id(branch.value.province)}`)
      .then((response) => {
        districts.value = response.data.data;
        wards.value = [];
        branch.value.district = "0";
        branch.value.ward = "0";
      })
      .catch((error) => {
        console.log(error);
      });
  }
}

function onDistrictChange() {
  if (branch.value.district) {
    axios
      .get(`/vietnam/wards/${get_district_id(branch.value.district)}`)
      .then((response) => {
        wards.value = response.data.data;
        branch.value.ward = "0";
      })
      .catch((error) => {
        console.log(error);
      });
  }
}

const fetchData = async () => {
  try {
    loading_data.value = true;
    const response = await axios.get("/branch/get-by-id", {
      params: {
        id: route.query.id,
      },
    });

    if (response.data.success) {
      branch.value = response.data.data;
      contact_old.value = branch.value.branch_contact_details.map((contact) => ({
        id: contact.id,
        name: get_icon_name(contact.branch_contact.id),
        contact_id: contact.branch_contact.id,
        contact_icon: get_icon_class(contact.branch_contact.id),
        value: contact.value,
      }));
      fetchDistricts();
    } else toast.error(response.data.message);
  } finally {
    loading_data.value = false;
  }
};

onMounted(() => {
  window.addEventListener("keydown", close_modal_delete);
  fetchBranchContact();
  fetchProvinces();
  fetchData();
});
</script>
