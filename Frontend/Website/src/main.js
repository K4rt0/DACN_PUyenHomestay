import "./assets/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import axios from "axios";
import VueAxios from "vue-axios";
import { createHead } from "@unhead/vue";

const app = createApp(App);
const head = createHead();

app.use(router);
axios.defaults.baseURL = "http://localhost:5169/api";
app.use(VueAxios, axios);
app.use(head);

app.mount("#app");
