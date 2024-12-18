<template>
  <aside id="layout-menu" class="layout-menu menu-vertical menu bg-menu-theme" data-bg-class="bg-menu-theme" style="touch-action: none; user-select: none; -webkit-user-drag: none; -webkit-tap-highlight-color: rgba(0, 0, 0, 0)">
    <div class="app-brand demo">
      <router-link :to="{ name: 'admin_dashboard' }" class="app-brand-link d-flex align-items-center justify-content-center">
        <span class="app-brand-logo demo justify-content-center">
          <img src="../../assets/logo_white.png" width="35" alt="" />
        </span>
        <span class="app-brand-text demo menu-text fw-bold ms-3">
          <img src="../../assets/logo_horizontal.png" width="130" alt="" />
        </span>
      </router-link>

      <a href="javascript:void(0);" class="layout-menu-toggle menu-link text-large ms-auto">
        <i class="bx bx-chevron-left bx-sm d-flex align-items-center justify-content-center"></i>
      </a>
    </div>

    <div class="menu-inner-shadow"></div>

    <ul class="menu-inner py-1 ps ps--active-y">
      <li v-if="role == 'root' || role == 'manager'" class="menu-item">
        <router-link :to="{ name: 'admin_dashboard' }" class="menu-link">
          <i class="menu-icon fa-duotone fa-solid fa-house"></i>
          <div class="text-truncate">Thống kê</div>
        </router-link>
      </li>
      <li v-if="role == 'root'" class="menu-item">
        <router-link :to="{ name: 'admin_staff_list' }" class="menu-link">
          <i class="menu-icon fa-sharp fa-solid fa-user-tie"></i>
          <div class="text-truncate">Danh sách quản lý</div>
        </router-link>
      </li>
      <li v-if="role == 'manager'" class="menu-item">
        <router-link :to="{ name: 'admin_manage_staff_list' }" class="menu-link">
          <i class="menu-icon fa-sharp fa-solid fa-user-tie"></i>
          <div class="text-truncate">Danh sách nhân viên</div>
        </router-link>
      </li>
      <li class="menu-item">
        <router-link :to="{ name: 'admin_customer_list' }" class="menu-link">
          <i class="menu-icon fa-regular fa-users"></i>
          <div class="text-truncate">Danh sách khách hàng</div>
        </router-link>
      </li>
      <li class="menu-item">
        <router-link :to="{ name: 'admin_reservation_list' }" class="menu-link">
          <i class="menu-icon fa-sharp-duotone fa-solid fa-receipt"></i>
          <div class="text-truncate">Đơn đặt phòng</div>
        </router-link>
      </li>

      <li v-if="role == 'root'" class="menu-header small text-uppercase"><span class="menu-header-text">Quản lý chi nhánh</span></li>

      <li v-if="role == 'root'" class="menu-item">
        <a class="menu-link menu-toggle">
          <i class="menu-icon fa-duotone fa-solid fa-code-branch fs-5"></i>
          <div class="text-truncate">Chi nhánh</div>
        </a>
        <ul class="menu-sub">
          <li class="menu-item">
            <router-link :to="{ name: 'admin_branch_contact_list' }" class="menu-link">
              <div class="text-truncate">Liên hệ chi nhánh</div>
            </router-link>
          </li>
          <li class="menu-item">
            <router-link :to="{ name: 'admin_branch_list' }" class="menu-link">
              <div class="text-truncate">Chi nhánh</div>
            </router-link>
          </li>
        </ul>
      </li>

      <li v-if="role == 'root' || role == 'manager'" class="menu-header small text-uppercase"><span class="menu-header-text">Quản lý phòng</span></li>

      <li v-if="role == 'root' || role == 'manager'" class="menu-item">
        <a class="menu-link menu-toggle">
          <i class="menu-icon fa-duotone fa-solid fa-bed fs-5"></i>
          <div class="text-truncate">Phòng ngủ</div>
        </a>
        <ul class="menu-sub">
          <li class="menu-item">
            <router-link :to="{ name: 'admin_room_facility' }" class="menu-link">
              <div class="text-truncate">Tiện nghi</div>
            </router-link>
          </li>
          <li class="menu-item">
            <router-link :to="{ name: 'admin_room_list' }" class="menu-link">
              <div class="text-truncate">Phòng ngủ</div>
            </router-link>
          </li>
        </ul>
      </li>
    </ul>
  </aside>
</template>

<script setup>
const role = localStorage.getItem("hotel_admin_role") ? localStorage.getItem("hotel_admin_role").toLowerCase() : "";
</script>
