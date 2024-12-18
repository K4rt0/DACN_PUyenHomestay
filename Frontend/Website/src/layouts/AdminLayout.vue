<template>
  <div class="layout-wrapper layout-content-navbar">
    <div class="layout-container">
      <AdminAsideComponent />
      <div class="layout-page">
        <AdminHeaderCompoent />
        <div class="content-wrapper">
          <div class="container-xxl flex-grow-1 container-p-y">
            <router-view />
          </div>
        </div>
      </div>
    </div>
    <div class="layout-overlay layout-menu-toggle"></div>
    <div class="drag-target"></div>
  </div>
  <Toaster class="me-5" position="top-right" richColors closeButton />
</template>

<style>
@import "/src/assets/fontawesome/css/all.css";
@import "/src/assets/fontawesome/css/sharp-duotone-solid.css";
@import "/src/assets/fontawesome/css/sharp-thin.css";
@import "/src/assets/fontawesome/css/sharp-solid.css";
@import "/src/assets/fontawesome/css/sharp-regular.css";
@import "/src/assets/fontawesome/css/sharp-light.css";
@import "/src/assets/admin/vendor/fonts/boxicons.css";
@import "/src/assets/admin/vendor/libs/apex-charts/apex-charts.css";
</style>

<script setup>
import AdminAsideComponent from "@/components/admin/AdminAsideComponent.vue";
import AdminHeaderCompoent from "@/components/admin/AdminHeaderCompoent.vue";
import { useHead } from "@unhead/vue";
import { onMounted } from "vue";
import { Toaster } from "vue-sonner";

useHead({
  title: "Phuong Uyen Homestay",
  htmlAttrs: {
    lang: "en",
    class: "light-style layout-navbar-fixed layout-menu-fixed layout-compact",
    dir: "ltr",
    "data-theme": "theme-default",
    "data-assets-path": "/src/assets/admin/",
    "data-template": "vertical-menu-template",
    "data-style": "dark",
  },
  link: [
    { href: "https://fonts.googleapis.com/css2?family=Public+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap", rel: "stylesheet" },
    { rel: "stylesheet", href: "/src/assets/admin/vendor/css/rtl/core-dark.css" },
    { rel: "stylesheet", href: "/src/assets/admin/vendor/css/rtl/theme-default-dark.css" },
    { rel: "stylesheet", href: "/src/assets/admin/css/demo.css" },
    { rel: "stylesheet", href: "/src/assets/admin/vendor/libs/perfect-scrollbar/perfect-scrollbar.css" },
  ],
  script: [
    { src: "/src/assets/admin/vendor/js/menu.js", async: true },
    { src: "/src/assets/admin/vendor/libs/apex-charts/apexcharts.js", async: true },
    { src: "/src/assets/admin/vendor/js/bootstrap.js", async: true },
    { src: "/src/assets/admin/vendor/js/helpers.js", async: true },
    { src: "/src/assets/admin/vendor/libs/perfect-scrollbar/perfect-scrollbar.js", async: true },
    { src: "/src/assets/admin/vendor/js/template-customizer.js", defer: true },
    { src: "/src/assets/admin/js/config.js", defer: true },
    { src: "/src/assets/admin/vendor/libs/popper/popper.js", defer: true },
  ],
});

onMounted(() => {
  setTimeout(() => {
    if (!document.querySelector(`script[src="/src/assets/admin/js/main.js"]`)) {
      const script = document.createElement("script");
      script.src = "/src/assets/admin/js/main.js";
      script.defer = true;
      document.head.appendChild(script);
    }
    if (!document.querySelector(`script[src="/src/assets/admin/js/charts-apex.js"]`)) {
      const script = document.createElement("script");
      script.src = "/src/assets/admin/js/charts-apex.js";
      script.defer = true;
      document.head.appendChild(script);
    }
  }, 1000);
});
</script>
