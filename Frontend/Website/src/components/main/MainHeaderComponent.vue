<template>
  <header class="reveal_header">
    <div class="container">
      <div class="row align-items-center">
        <div class="col-6">
          <router-link :to="{ name: 'home' }" class="logo_normal"><img src="../../assets/logo_horizontal.png" width="189" height="44" alt="" /></router-link>
          <router-link :to="{ name: 'home' }" class="logo_sticky"><img src="../../assets/logo_sticky.png" width="189" height="44" alt="" /></router-link>
        </div>
        <div class="col-6">
          <nav>
            <ul>
              <li>
                <div class="hamburger_2 open_close_nav_panel">
                  <div class="hamburger__box">
                    <div class="hamburger__inner"></div>
                  </div>
                </div>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
    <!-- /container -->
  </header>
  <!-- /Header -->

  <div class="nav_panel">
    <a href="#0" class="closebt open_close_nav_panel"><i class="bi bi-x"></i></a>
    <div class="logo_panel"><img src="../../assets/logo_sticky.png" width="189" height="44" alt="" /></div>
    <div class="sidebar-navigation">
      <nav>
        <ul class="level-1">
          <li><router-link :to="{ name: 'home' }">Trang chủ</router-link></li>
          <li class="parent">
            <a href="#">Tài khoản</a>
            <ul class="level-2">
              <li class="back"><a href="#">Back</a></li>
              <template v-if="!is_logged">
                <li><router-link :to="{ name: 'main_login' }">Đăng nhập</router-link></li>
                <li><router-link :to="{ name: 'register' }">Đăng ký</router-link></li>
              </template>
              <template v-else>
                <li><router-link :to="{ name: 'profile' }">Thông tin cá nhân</router-link></li>
                <li><a href="#" @click="logout">Đăng xuất</a></li>
              </template>
            </ul>
          </li>
        </ul>
        <div class="panel_footer">
          <div class="phone_element">
            <a href="tel://423424234">
              <i class="bi bi-telephone"></i>
              <span><em>Info and bookings</em>+41 934 121 1334</span>
            </a>
          </div>
        </div>
        <!-- /panel_footer -->
      </nav>
    </div>
    <!-- /sidebar-navigation -->
  </div>
  <!-- /nav_panel -->
</template>

<script setup>
import { ref, onMounted } from "vue";

const is_logged = ref(false);

const logout = () => {
  localStorage.removeItem("jwt_token_website");
  window.location.href = "/";
};

onMounted(() => {
  if (localStorage.getItem("jwt_token_website")) is_logged.value = true;
});
</script>
