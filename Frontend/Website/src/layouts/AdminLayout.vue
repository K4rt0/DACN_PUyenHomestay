<template>
  <div class="d-flex flex-column flex-root app-root" id="kt_app_root">
    <div class="app-page flex-column flex-column-fluid" id="kt_app_page">
      <AdminHeaderCompoent />
      <div class="app-wrapper flex-column flex-row-fluid" id="kt_app_wrapper">
        <AdminAsideComponent />
        <div class="app-main flex-column flex-row-fluid" id="kt_app_main">
          <div class="d-flex flex-column flex-column-fluid">
            <router-view />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
@import "@/assets/plugins/custom/fullcalendar/fullcalendar.bundle.css";
@import "@/assets/plugins/custom/datatables/datatables.bundle.css";
@import "@/assets/plugins/global/plugins.bundle.css";
@import "@/assets/css/style.bundle.css";
</style>

<script setup>
import AdminAsideComponent from "@/components/admin/AdminAsideComponent.vue";
import AdminHeaderCompoent from "@/components/admin/AdminHeaderCompoent.vue";
import { useHead } from "@unhead/vue";
import { onBeforeMount } from "vue";

onBeforeMount(() => {
  useHead({
    title: "Admin Layout",
    meta: [
      { property: "og:locale", content: "en_US" },
      { property: "og:type", content: "article" },
      {
        property: "og:title",
        content: "Metronic - The World's #1 Selling Tailwind CSS & Bootstrap Admin Template by KeenThemes",
      },
      { property: "og:url", content: "https://keenthemes.com/metronic" },
      { property: "og:site_name", content: "Metronic by Keenthemes" },
    ],
    link: [
      { rel: "canonical", href: "index.html" },
      { rel: "shortcut icon", href: "/assets/media/logos/favicon.ico" },
      {
        rel: "stylesheet",
        href: "https://fonts.googleapis.com/css?family=Inter:300,400,500,600,700",
      },
    ],
    bodyAttrs: {
      id: "kt_app_body",
      "data-kt-app-layout": "dark-sidebar",
      "data-kt-app-header-fixed": "true",
      "data-kt-app-sidebar-enabled": "true",
      "data-kt-app-sidebar-fixed": "true",
      "data-kt-app-sidebar-hoverable": "true",
      "data-kt-app-sidebar-push-header": "true",
      "data-kt-app-sidebar-push-toolbar": "true",
      "data-kt-app-sidebar-push-footer": "true",
      "data-kt-app-toolbar-enabled": "true",
      class: "app-default",
    },
    script: [{ src: "/src/assets/plugins/global/plugins.bundle.js", async: false, onload: loadScriptsBundle }],
  });
});

function loadScriptsBundle() {
  const script = document.createElement("script");
  script.src = "/src/assets/js/scripts.bundle.js";
  script.defer = true;
  document.body.appendChild(script);
}
</script>
