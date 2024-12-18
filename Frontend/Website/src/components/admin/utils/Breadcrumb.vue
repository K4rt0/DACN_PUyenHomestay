<template>
  <div class="navbar-nav align-items-center">
    <div class="nav-item navbar-search-wrapper">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item">
          <router-link :to="{ name: 'admin_home' }">Trang chính</router-link>
        </li>
        <template v-for="(page, index) in breadcrumbs" :key="index">
          <li v-if="index < breadcrumbs.length - 1" class="breadcrumb-item">
            <router-link :to="{ name: page.path }">{{ page.name }}</router-link>
          </li>
          <li v-else class="breadcrumb-item active">{{ page.name }}</li>
        </template>
      </ol>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";
import { useBreadcrumbStore } from "@/stores/useBreadcrumbStore";

const breadcrumbStore = useBreadcrumbStore();
const breadcrumbs = computed(() => breadcrumbStore.getBreadcrumbs);
</script>
