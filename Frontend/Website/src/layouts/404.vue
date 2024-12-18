<template>
  <main>
    <div class="container">
      <div class="row error_page">
        <img class="col-md-6 align-self-center" src="../assets/main/img/404.svg" alt="" />
        <div class="col-md-6 align-self-center">
          <h1>404</h1>
          <h2>Thôi xong, bạn đã bị lạc rồi.</h2>
          <p>Trang bạn đang tìm kiếm không tồn tại. Nhưng bạn có thể nhấp vào nút bên dưới để quay lại trang chủ.</p>
          <p>
            <a href="/" class="btn_1 mt-3">Trở lại trang chủ</a>
          </p>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup>
import { useHead } from "@unhead/vue";

useHead({
  title: "404",
  link: [
    { rel: "preconnect", href: "https://fonts.googleapis.com" },
    { rel: "preconnect", href: "https://fonts.gstatic.com", crossorigin: true },
    { href: "https://fonts.googleapis.com/css2?family=Caveat:wght@400;500&family=Montserrat:wght@300;400;500;600;700&display=swap", rel: "stylesheet" },
    { href: "/src/assets/main/css/bootstrap.min.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/style.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/vendors.min.css", rel: "stylesheet" },
    { href: "/src/assets/main/css/custom.css", rel: "stylesheet" },
  ],
});
</script>
