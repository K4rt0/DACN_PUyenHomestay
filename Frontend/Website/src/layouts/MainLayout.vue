<template>
  <div class="layer"></div>
  <!-- Opacity Mask -->

  <MainHeaderComponent />

  <router-view />

  <MainFooterComponent />
  <!-- /footer -->

  <div class="progress-wrap">
    <svg width="100%" height="100%" viewbox="-1 -1 102 102">
      <path d="M50,1 a49,49 0 0,1 0,98 a49,49 0 0,1 0,-98"></path>
    </svg>
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
</style>

<script setup>
import { onMounted } from "vue";
import { useHead } from "@unhead/vue";
import MainHeaderComponent from "@/components/main/MainHeaderComponent.vue";
import MainFooterComponent from "@/components/main/MainFooterComponent.vue";
import { Toaster } from "vue-sonner";

!(function (s, u, b, i, z) {
  var o, t, r, y;
  s[i] ||
    ((s._sbzaccid = z),
    (s[i] = function () {
      s[i].q.push(arguments);
    }),
    (s[i].q = []),
    s[i]("setAccount", z),
    (r = [
      "widget.subiz.net",
      "storage.googleapis" + (t = ".com"),
      "app.sbz.workers.dev",
      i +
        "a" +
        (o = function (k, t) {
          var n = t <= 6 ? 5 : o(k, t - 1) + o(k, t - 3);
          return k !== t ? n : n.toString(32);
        })(20, 20) +
        t,
      i + "b" + o(30, 30) + t,
      i + "c" + o(40, 40) + t,
    ]),
    (y = function (k) {
      var t, n;
      s._subiz_init_2094850928430 || (r[k] && ((t = u.createElement(b)), (n = u.getElementsByTagName(b)[0]), (t.async = 1), (t.src = "https://" + r[k] + "/sbz/app.js?accid=" + z), n.parentNode.insertBefore(t, n), setTimeout(y, 2e3, k + 1)));
    })(0));
})(window, document, "script", "subiz", "acseskyvupoyuxspbuxm");
useHead({
  link: [
    { rel: "apple-touch-icon", type: "image/x-icon", href: "/src/assets/main/img/apple-touch-icon-57x57-precomposed.png" },
    { rel: "apple-touch-icon", type: "image/x-icon", sizes: "72x72", href: "/src/assets/main/img/apple-touch-icon-72x72-precomposed.png" },
    { rel: "apple-touch-icon", type: "image/x-icon", sizes: "114x114", href: "/src/assets/main/img/apple-touch-icon-114x114-precomposed.png" },
    { rel: "apple-touch-icon", type: "image/x-icon", sizes: "144x144", href: "/src/assets/main/img/apple-touch-icon-144x144-precomposed.png" },
    { rel: "preconnect", href: "https://fonts.googleapis.com" },
    { rel: "preconnect", href: "https://fonts.gstatic.com", crossorigin: true },
    { href: "https://fonts.googleapis.com/css2?family=Caveat:wght@400;500&family=Montserrat:wght@300;400;500;600;700&display=swap", rel: "stylesheet" },
    { href: "/src/assets/main/css/bootstrap.min.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/style.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/vendors.min.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/custom.css", rel: "stylesheet" },
  ],
});

onMounted(() => {
  const script = document.createElement("script");
  script.src = "/src/assets/main/js/common_scripts.js";
  script.async = true;
  script.onload = () => {
    import("/src/assets/main/js/common_functions.js");
    import("/src/assets/main/js/slider.js");
  };
  document.head.appendChild(script);
});
</script>
