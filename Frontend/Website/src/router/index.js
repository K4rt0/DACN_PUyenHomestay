import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: "/admin", redirect: "/admin/dashboard" },
    {
      path: "/admin",
      component: () => import("../layouts/AdminLayout.vue"),
      children: [
        {
          path: "dashboard",
          name: "admin_dashboard",
          component: () => import("../views/admin/Dashboard.vue"),
        },
      ],
    },
  ],
});

export default router;
