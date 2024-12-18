import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import axios from "axios";
import VueAxios from "vue-axios";
import { createHead } from "@unhead/vue";
import { createPinia } from "pinia";

const app = createApp(App);
const head = createHead();

app.use(router);
app.use(VueAxios, axios);
app.use(createPinia());
app.use(head);

app.mount("#app");
