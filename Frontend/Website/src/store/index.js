// src/store.js
import { createStore } from "vuex";

export default createStore({
  state: {
    breadcrumbs: [],
  },
  mutations: {
    setBreadcrumbs(state, breadcrumbs) {
      state.breadcrumbs = breadcrumbs;
    },
  },
  actions: {
    updateBreadcrumbs({ commit }, breadcrumbs) {
      commit("setBreadcrumbs", breadcrumbs);
    },
  },
  getters: {
    getBreadcrumbs(state) {
      return state.breadcrumbs;
    },
  },
});
