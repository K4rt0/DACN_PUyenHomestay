import { createRouter, createWebHistory } from "vue-router";
import axios from "@/utils/axios";

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
        // Branch
        {
          path: "branch-list",
          name: "admin_branch_list",
          component: () => import("../views/admin/branch/BranchList.vue"),
        },
        {
          path: "branch-new",
          name: "admin_branch_new",
          component: () => import("../views/admin/branch/BranchNew.vue"),
        },
        {
          path: "branch-update",
          name: "admin_branch_update",
          component: () => import("../views/admin/branch/BranchUpdate.vue"),
        },
        // Branch Contact
        {
          path: "branch-contact-list",
          name: "admin_branch_contact_list",
          component: () => import("../views/admin/branch-contact/BranchContactList.vue"),
        },
      ],
    },
    {
      path: "/admin",
      component: () => import("../layouts/AdminAuthLayout.vue"),
      children: [
        {
          path: "login",
          name: "admin_auth",
          component: () => import("../views/admin/auth/AdminAuth.vue"),
        },
      ],
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  if (to.path.startsWith("/admin") && to.path !== "/admin/login") {
    try {
      const token = localStorage.getItem("jwt_admin_token");

      if (token) {
        const response = await axios.get("/admin/auth", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.data.success) next();
        else next("/admin/login");
      } else next("/admin/login");
    } catch (error) {
      await router.push("/admin/login");
      setTimeout(() => {
        localStorage.removeItem("admin_user_data");
        localStorage.removeItem("jwt_admin_token");
        localStorage.removeItem("admin_login_success");
      }, 100);
      next();
    }
  } else {
    next();
  }
});

export default router;
