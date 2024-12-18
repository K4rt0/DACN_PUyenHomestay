import { useBreadcrumbStore } from "@/stores/useBreadcrumbStore";

export default function breadcrumb(routers) {
  const breadcrumbStore = useBreadcrumbStore();
  breadcrumbStore.updateBreadcrumbs(routers);
}
