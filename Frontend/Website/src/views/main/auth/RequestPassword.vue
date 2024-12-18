<template>
  <div class="hero home-search full-height bg-dark">
    <div class="container h-100">
      <div class="d-flex justify-content-center align-items-center h-100">
        <div class="card w-50" style="background-color: #e0e0e0">
          <div class="card-body">
            <h3 class="text-center py-2">Đặt mật khẩu</h3>
            <form class="p-3" @submit.prevent="submit_password">
              <div class="row">
                <div class="col-12">
                  <div class="form-group">
                    <label class="text-muted" for="password">Mật khẩu</label>
                    <input type="password" class="form-control fs-4" id="password" placeholder="························" v-model="password" />
                  </div>
                </div>
                <div class="col-12">
                  <div class="form-group">
                    <label class="text-muted" for="password">Nhập lại mật khẩu</label>
                    <input type="password" class="form-control fs-4" id="retype-password" placeholder="························" v-model="retype_password" />
                  </div>
                </div>
              </div>
              <div class="d-flex justify-content-center">
                <button type="submit" class="btn btn-success">Xác nhận</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import axios from "@/utils/axios";
import { ref } from "vue";
import { toast } from "vue-sonner";

const password = ref("");
const retype_password = ref("");

const submit_password = async () => {
  if (password.value === "" || retype_password.value === "") {
    toast.warning("Vui lòng nhập mật khẩu và nhập lại mật khẩu !");
  } else if (password.value === retype_password.value) {
    const response = await axios.put("/user/request-password", null, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_token_website")}`,
      },
      params: {
        password: password.value,
        retype_password: retype_password.value,
      },
    });

    if (response.data.success) {
      toast.success(response.data.message);
      setTimeout(() => {
        window.location.href = "/";
      }, 1000);
    } else {
      toast.error(response.data.message);
    }
  } else {
    toast.warning("Mật khẩu không khớp !");
  }
};
</script>
