import { useStore } from "vuex";

export default function breadcrumb(routers) {
  const store = useStore();
  store.dispatch("updateBreadcrumbs", routers);
}
