import { defineStore } from "pinia";

export const useBreadcrumbStore = defineStore("breadcrumbStore", {
  state: () => ({
    breadcrumbs: [],
  }),

  actions: {
    updateBreadcrumbs(breadcrumbs) {
      this.breadcrumbs = breadcrumbs;
    },
  },

  getters: {
    getBreadcrumbs: (state) => state.breadcrumbs,
  },
});
