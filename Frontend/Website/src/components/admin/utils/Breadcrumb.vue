<template>
  <div class="navbar-nav align-items-center">
    <div class="nav-item navbar-search-wrapper">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item">
          <router-link :to="{ name: 'admin_dashboard' }">Home</router-link>
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
import { useStore } from "vuex";
import { computed } from "vue";

const store = useStore();

const breadcrumbs = computed(() => store.getters.getBreadcrumbs);
</script>
