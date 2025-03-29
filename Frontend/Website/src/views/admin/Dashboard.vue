<template>
  <div class="card">
    <div class="card-header d-flex justify-content-between">
      <div>
        <h5 class="card-title mb-0">Doanh thu</h5>
        <p class="card-subtitle my-0">Các đơn đặt phòng đã hoàn thành</p>
        <p class="card-subtitle my-0">Tổng doanh thu của năm {{ year }}: {{ sum.toLocaleString("vi-VN", { style: "currency", currency: "VND" }) }}</p>
      </div>
      <div>
        <select v-model="year" class="form-select">
          <option v-for="year in years" :key="year" :value="year">{{ year }}</option>
        </select>
      </div>
    </div>
    <div class="card-body">
      <div id="homestay-lineAreaChart"></div>
    </div>
  </div>
</template>

<script setup>
import axios from "@/utils/axios";
import { onMounted, ref, watch } from "vue";
import breadcrumb from "@/utils/breadcrumb";

breadcrumb([{ path: "", name: "Trang chính" }]);
breadcrumb([{ path: "dashboard", name: "Thống kê" }]);

const currentYear = new Date().getFullYear();
const years = [];
for (let year = currentYear; year >= currentYear - 3; year--) {
  years.push(year);
}
const year = ref(currentYear);
const months = ref([]);
const chart = ref(null);
const sum = ref(0);

const updateChart = async () => {
  try {
    const response = await axios.get("/reservation/statistical", {
      params: {
        year: year.value,
      },
      headers: {
        Authorization: `Bearer ${localStorage.getItem("jwt_admin_token")}`,
      },
    });

    if (response.data.success) {
      console.log(response.data.data);
      months.value = [];
      for (let i = 1; i <= 12; i++) {
        months.value.push(`${i.toString().padStart(2, "0")}/${year.value}`);
      }
      if (chart.value) {
        chart.value.updateOptions({
          xaxis: {
            categories: months.value,
          },
          series: response.data.data,
        });
        sum = response.data.data.map((sum.value = response.data.data.reduce((acc, curr) => acc + curr.data.reduce((a, b) => a + b, 0), 0)));
      }
    }
  } catch (error) {}
};

watch(year, updateChart);

onMounted(() => {
  const i = {
    series1: "#29dac7",
    series2: "#60f2ca",
    series3: "#a5f8cd",
  };
  var d = document.querySelector("#homestay-lineAreaChart"),
    h = {
      chart: { height: 400, type: "line", parentHeightOffset: 0, zoom: { enabled: !1 }, toolbar: { show: !1 } },
      series: [],
      markers: { strokeWidth: 7, strokeOpacity: 1, strokeColors: ["#2b2c40"], colors: "#b2b2c4" },
      dataLabels: { enabled: !1 },
      stroke: { curve: "straight" },
      colors: [i.series3, i.series2, i.series1],
      legend: { show: !0, position: "bottom", horizontalAlign: "center", labels: { colors: "#b2b2c4" } },
      grid: { borderColor: "#4e4f6c", xaxis: { lines: { show: !0 } }, padding: { top: -20 } },

      xaxis: {
        categories: months.value,
        axisBorder: { show: !1 },
        axisTicks: { show: !1 },
        labels: {
          style: {
            colors: "#7e7f96",
            fontSize: "13px",
          },
        },
      },
      yaxis: { labels: { style: { colors: "#7e7f96", fontSize: "13px" } } },
    };
  const checkApexCharts = setInterval(() => {
    if (typeof ApexCharts !== "undefined") {
      clearInterval(checkApexCharts);
      chart.value = new ApexCharts(d, h);
      chart.value.render();
      year.value = currentYear;
      updateChart();
    }
  }, 100);
});
</script>
