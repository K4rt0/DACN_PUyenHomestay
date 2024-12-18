<template>
  <nav class="layout-navbar container-xxl navbar navbar-expand-xl navbar-detached align-items-center bg-navbar-theme" id="layout-navbar">
    <div class="layout-menu-toggle navbar-nav align-items-xl-center me-4 me-xl-0 d-xl-none">
      <a class="nav-item nav-link px-0 me-xl-6" href="javascript:void(0)">
        <i class="bx bx-menu bx-md"></i>
      </a>
    </div>

    <div class="navbar-nav-right d-flex align-items-center" id="navbar-collapse">
      <Breadcrumb />

      <ul class="navbar-nav flex-row align-items-center ms-auto">
        <!-- User -->
        <li class="nav-item navbar-dropdown dropdown-user dropdown">
          <a class="nav-link dropdown-toggle hide-arrow p-0" data-bs-toggle="dropdown">
            <div class="avatar avatar-online">
              <img src="../../assets/admin/img/avatars/1.png" alt="" class="w-px-40 h-auto rounded-circle" />
            </div>
          </a>
          <ul class="dropdown-menu dropdown-menu-end">
            <li>
              <a class="dropdown-item" href="pages-account-settings-account.html">
                <div class="d-flex">
                  <div class="flex-shrink-0 me-3">
                    <div class="avatar avatar-online">
                      <img src="../../assets/admin/img/avatars/1.png" alt="" class="w-px-40 h-auto rounded-circle" />
                    </div>
                  </div>
                  <div class="flex-grow-1">
                    <h6 class="mb-0">{{ is_root() ? "Root Account" : user?.full_name }}</h6>
                    <small class="text-muted">{{ is_root() ? "Root" : user?.role }}</small>
                  </div>
                </div>
              </a>
            </li>
            <li>
              <div class="dropdown-divider my-1"></div>
            </li>
            <li>
              <button class="dropdown-item" @click="logout"><i class="bx bx-power-off bx-md me-3"></i><span>Log Out</span></button>
            </li>
          </ul>
        </li>
        <!--/ User -->
      </ul>
    </div>

    <!-- Search Small Screens -->
    <div class="navbar-search-wrapper search-input-wrapper d-none">
      <input type="text" class="form-control search-input container-xxl border-0" placeholder="Search..." aria-label="Search..." />
      <i class="bx bx-x bx-md search-toggler cursor-pointer"></i>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from "vue";
import Breadcrumb from "./utils/Breadcrumb.vue";
import axios from "@/utils/axios";

const user = ref(null);

const fetchUser = async () => {
  const jwt_token = localStorage.getItem("jwt_admin_token");
  if (jwt_token) {
    const response = await axios.get("/admin/me", {
      headers: {
        Authorization: `Bearer ${jwt_token}`,
      },
    });
    if (response.data.success) {
      user.value = response.data.data;
      localStorage.setItem("hotel_admin_role", response.data.data.role);
    } else {
      localStorage.removeItem("jwt_admin_token");
      localStorage.removeItem("hotel_admin_role");
      window.location.href = "/admin/login";
    }
  }
};

onMounted(() => {
  fetchUser();
});

const is_root = () => {
  const role = localStorage.getItem("hotel_admin_role");
  return role && role.toLowerCase() === "root";
};
const logout = () => {
  localStorage.removeItem("jwt_admin_token");
  localStorage.removeItem("hotel_admin_role");
  window.location.href = "/admin/login";
};
</script>
