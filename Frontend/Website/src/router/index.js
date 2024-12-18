import { createRouter, createWebHistory } from "vue-router";
import axios from "@/utils/axios";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "",
      component: () => import("../layouts/MainLayout.vue"),
      children: [
        {
          path: "",
          name: "home",
          component: () => import("../views/main/Home.vue"),
        },
        {
          path: "room-search",
          name: "room-search",
          component: () => import("../views/main/room/RoomList.vue"),
        },
        {
          path: "room-booking",
          name: "room-booking",
          component: () => import("../views/main/room/RoomBooking.vue"),
        },
        {
          path: "payment-return",
          name: "payment-return",
          component: () => import("../views/main/room/PaymentReturn.vue"),
        },
        {
          path: "booking/:id",
          name: "booking",
          component: () => import("../views/main/room/BookingDetail.vue"),
        },
        // User Profile
        {
          path: "profile",
          name: "profile",
          component: () => import("../views/main/user/UserInfo.vue"),
        },
        // Authenticated User
        {
          path: "login",
          name: "main_login",
          component: () => import("../views/main/auth/Login.vue"),
        },
        {
          path: "register",
          name: "register",
          component: () => import("../views/main/auth/Register.vue"),
        },
        {
          path: "request-pwd",
          name: "request-pwd",
          component: () => import("../views/main/auth/RequestPassword.vue"),
        },
        {
          path: "verify-account",
          name: "verify-account",
          component: () => import("../views/main/auth/VerifyAccount.vue"),
        },
      ],
    },

    // Admin Router
    {
      path: "/admin",
      component: () => import("../layouts/AdminLayout.vue"),
      children: [
        {
          path: "",
          name: "admin_home",
        },
        {
          path: "dashboard",
          name: "admin_dashboard",
          component: () => import("../views/admin/Dashboard.vue"),
          meta: {
            requiresRole: "manager",
          },
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
        // Room control
        {
          path: "room-facility",
          name: "admin_room_facility",
          component: () => import("../views/admin/facility/FacilityList.vue"),
        },
        {
          path: "room-list",
          name: "admin_room_list",
          component: () => import("../views/admin/room/RoomList.vue"),
        },
        {
          path: "room-new",
          name: "admin_room_new",
          component: () => import("../views/admin/room/RoomNew.vue"),
        },
        {
          path: "room-update",
          name: "admin_room_update",
          component: () => import("../views/admin/room/RoomUpdate.vue"),
        },
        // Reservation
        {
          path: "reservation-list",
          name: "admin_reservation_list",
          component: () => import("../views/admin/reservation/ReservationList.vue"),
        },
        {
          path: "reservation-view",
          name: "admin_reservation_view",
          component: () => import("../views/admin/reservation/ReservationView.vue"),
        },
        // User Manager
        {
          path: "customer-list",
          name: "admin_customer_list",
          component: () => import("../views/admin/user/CustomerList.vue"),
        },
        {
          path: "customer-view",
          name: "admin_customer_view",
          component: () => import("../views/admin/user/CustomerView.vue"),
        },
        {
          path: "staff-list",
          name: "admin_staff_list",
          component: () => import("../views/admin/user/StaffRootList.vue"),
          meta: {
            requiresRole: "root",
          },
        },
        {
          path: "staff-view",
          name: "admin_staff_view",
          component: () => import("../views/admin/user/StaffRootView.vue"),
          meta: {
            requiresRole: "root",
          },
        },
        {
          path: "staff-add",
          name: "admin_staff_add",
          component: () => import("../views/admin/user/StaffRootAdd.vue"),
          meta: {
            requiresRole: "root",
          },
        },
        {
          path: "manage-staff",
          name: "admin_manage_staff_list",
          component: () => import("../views/admin/user/StaffList.vue"),
          meta: {
            requiresRole: "manager",
          },
        },
        {
          path: "manage-staff-view",
          name: "admin_manage_staff_view",
          component: () => import("../views/admin/user/StaffView.vue"),
          meta: {
            requiresRole: "manager",
          },
        },
        {
          path: "manage-staff-add",
          name: "admin_manage_staff_add",
          component: () => import("../views/admin/user/StaffAdd.vue"),
          meta: {
            requiresRole: "manager",
          },
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

    // 404 Layout
    {
      path: "/:pathMatch(.*)*",
      component: () => import("../layouts/404.vue"),
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  if (to.meta.requiresRole) {
    const role = localStorage.getItem("hotel_admin_role");
    if (role.toLowerCase() !== to.meta.requiresRole.toLowerCase() && !(role.toLowerCase() === "root" && to.meta.requiresRole.toLowerCase() === "manager")) {
      if (localStorage.getItem("jwt_admin_token") == null) return next("/admin/login");

      return next({ path: "/404" });
    }
  }
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
        localStorage.removeItem("jwt_admin_token");
        localStorage.setItem("is_root_admin", false);
      }, 100);
      next();
    }
  } else if (to.path !== "/login" && to.path !== "/register" && to.path !== "/request-pwd" && !to.path.startsWith("/admin")) {
    const token = localStorage.getItem("jwt_token_website");
    if (token) {
      try {
        const response = await axios.get("/user/is-change-pwd", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (response.data.success) {
          next("/request-pwd");
        } else {
          next();
        }
      } catch (error) {
        localStorage.removeItem("jwt_token_website");
        next();
      }
    } else {
      localStorage.removeItem("jwt_token_website");
      next();
    }
  } else {
    next();
  }
});

export default router;
