<template>
  <input id="datepicker" v-model="date" />
</template>

<script setup>
import { ref, onMounted, watch } from "vue";
import { easepick, RangePlugin, LockPlugin, DateTime } from "@easepick/bundle";
let picker;
const emit = defineEmits(["update:modelValue"]);
const props = defineProps({
  modelValue: String,
});
const date = ref(props.modelValue);

onMounted(() => {
  const dates = [["2023-09-01", "2023-09-04"], "2023-09-07", ["2023-10-11", "2023-10-17"]].map((date) => {
    if (date instanceof Array) {
      const start = new DateTime(date[0], "YYYY-MM-DD");
      const end = new DateTime(date[1], "YYYY-MM-DD");
      return [start, end];
    }
    return new DateTime(date, "YYYY-MM-DD");
  });

  picker = new easepick.create({
    element: "#datepicker",
    css: ["https://cdn.jsdelivr.net/npm/@easepick/bundle@1.2.1/dist/index.css"],
    lang: "en-EN",
    format: "MM/DD/YYYY",
    calendars: 2,
    grid: 2,
    zIndex: 10,
    plugins: [LockPlugin, RangePlugin],
    RangePlugin: {
      tooltipNumber: function (number) {
        return number - 1;
      },
      locale: { one: "đêm", other: "đêm" },
      minDays: 1,
      maxDays: 30,
      selectForward: true,
      selectBackward: false,
    },
    LockPlugin: {
      minDate: new Date(),
      minDays: 1,
      inseparable: false,
      filter: function (date, pickedDates) {
        if (pickedDates.length === 1) {
          const rangeType = date.isBefore(pickedDates[0]) ? "]" : "[";
          return !pickedDates[0].isSame(date, "day") && date.inArray(dates, rangeType);
        }
        return date.inArray(dates, "]");
      },
    },
  });

  picker.on("select", (dates) => {
    date.value = `${dates.detail.start.format("MM/DD/YYYY")} - ${dates.detail.end.format("MM/DD/YYYY")}`;
    emit("update:modelValue", date.value);
  });

  // Set initial value
  if (props.modelValue) {
    const [start, end] = props.modelValue.split(" - ");
    picker.setDateRange(start, end);
  }
});

watch(
  () => props.modelValue,
  (newValue) => {
    date.value = newValue;
    if (newValue) {
      const [start, end] = newValue.split(" - ");
      picker.setDateRange(start, end);
    }
  }
);
</script>
