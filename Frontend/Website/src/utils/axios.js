import axios1 from "axios";
import { toast } from "vue-sonner";

const axios = axios1.create({
  baseURL: "http://localhost:5043/api",
});

axios.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    toast.error("Đã có lỗi xảy ra trong quá trình lấy dữ liệu. Vui lòng liên hệ Admin Karto thông qua Zalo: 0949.598.227");
    return Promise.reject(error);
  }
);

export default axios;
