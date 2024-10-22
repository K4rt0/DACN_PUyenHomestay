<template>
  <div class="container-xxl">
    <div class="authentication-wrapper authentication-basic container-p-y">
      <div class="authentication-inner">
        <router-view />
      </div>
    </div>
  </div>
  <Toaster class="me-5" position="top-right" richColors closeButton />
</template>

<style>
/* Fontawesome */
@import "/src/assets/fontawesome/css/all.css";
@import "/src/assets/fontawesome/css/sharp-duotone-solid.css";
@import "/src/assets/fontawesome/css/sharp-thin.css";
@import "/src/assets/fontawesome/css/sharp-solid.css";
@import "/src/assets/fontawesome/css/sharp-regular.css";
@import "/src/assets/fontawesome/css/sharp-light.css";

/* Box Icons */
@import "/src/assets/admin/vendor/fonts/boxicons.css";

/* CSS */
@import "/src/assets/admin/vendor/css/rtl/core-dark.css";
@import "/src/assets/admin/vendor/css/rtl/theme-default-dark.css";
@import "/src/assets/admin/css/demo.css";
@import "/src/assets/admin/vendor/libs/perfect-scrollbar/perfect-scrollbar.css";
@import "/src/assets/admin/vendor/css/pages/page-auth.css";
</style>

<script setup>
import { useHead } from "@unhead/vue";
import { onMounted } from "vue";
import { Toaster } from "vue-sonner";

useHead({
  title: "Admin Layout",
  htmlAttrs: {
    lang: "en",
    class: "light-style layout-wide customizer-hide",
    dir: "ltr",
    "data-theme": "theme-default",
    "data-assets-path": "/src/assets/admin/",
    "data-template": "vertical-menu-template",
    "data-style": "dark",
  },
  link: [
    { rel: "icon", type: "image/x-icon", href: "https://demos.themeselection.com/sneat-bootstrap-html-admin-template/assets/img/favicon/favicon.ico" },
    { href: "https://fonts.googleapis.com/css2?family=Public+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&display=swap", rel: "stylesheet" },
  ],
  script: [
    { src: "/src/assets/admin/vendor/js/menu.js", async: true },
    { src: "/src/assets/admin/vendor/js/bootstrap.js", async: true },
    { src: "/src/assets/admin/vendor/js/helpers.js", async: true },
    { src: "/src/assets/admin/vendor/libs/perfect-scrollbar/perfect-scrollbar.js", async: true },
    { src: "/src/assets/admin/vendor/js/template-customizer.js", defer: true },
    { src: "/src/assets/admin/js/config.js", defer: true },
    { src: "/src/assets/admin/vendor/libs/@form-validation/auto-focus.js", defer: true },
    { src: "/src/assets/admin/js/pages-auth.js", defer: true },
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
  }, 1000);
});
</script>
