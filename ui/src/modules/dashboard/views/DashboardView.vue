<template>
  <div
    v-if="loading && isFirstLoad"
    class="flex h-96 items-center justify-center"
  >
    <div
      class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-[#6366f1]"
    ></div>
  </div>

  <div
    v-else
    class="flex flex-col flex-1 space-y-6 animate-in fade-in duration-500"
  >
    <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-5 gap-4">
      <div class="kpi-card">
        <p class="label">$ SALDO INICIAL</p>
        <p class="value text-[#2dd4bf]">
          {{ formatBRL(dashboardData.saldoInicial) }}
        </p>
      </div>

      <div class="kpi-card">
        <p class="label">$ RECEITAS</p>
        <p class="value text-[#10b981]">
          {{ formatBRL(dashboardData.totalReceitas) }}
        </p>
      </div>

      <div class="kpi-card">
        <p class="label">$ DESPESAS</p>
        <p class="value text-[#ef4444]">
          {{ formatBRL(dashboardData.totalDespesas) }}
        </p>
      </div>

      <div class="kpi-card">
        <p class="label">$ SALDO ATUAL</p>
        <p class="value text-[#2dd4bf]">
          {{ formatBRL(dashboardData.saldoAtual) }}
        </p>
      </div>

      <div class="relative">
        <div
          @click="toggleDatePicker"
          class="bg-[#18181b] p-5 rounded-2xl border border-white/5 flex flex-col justify-center items-center cursor-pointer hover:bg-white/5 transition-colors h-full"
        >
          <p class="label">PERÍODO</p>
          <p class="text-white font-black text-lg flex items-center gap-2">
            {{ formattedPeriod }}
            <span
              class="text-xs transition-transform duration-300"
              :class="{ 'rotate-180': showDatePicker }"
              >▾</span
            >
          </p>
        </div>

        <div
          v-if="showDatePicker"
          @click="showDatePicker = false"
          class="fixed inset-0 z-40"
        ></div>

        <div
          v-if="showDatePicker"
          class="absolute top-[calc(100%+8px)] right-0 w-64 bg-[#18181b] border border-white/5 rounded-2xl shadow-2xl p-4 z-50 animate-in slide-in-from-top-2 fade-in duration-200"
        >
          <div
            class="flex justify-between items-center mb-4 border-b border-white/5 pb-4"
          >
            <button
              @click="currentYearView--"
              class="text-gray-400 hover:text-white px-2 py-1 rounded hover:bg-white/5 transition-colors"
            >
              ❮
            </button>
            <span class="text-white font-bold">{{ currentYearView }}</span>
            <button
              @click="currentYearView++"
              class="text-gray-400 hover:text-white px-2 py-1 rounded hover:bg-white/5 transition-colors"
            >
              ❯
            </button>
          </div>

          <div class="grid grid-cols-3 gap-2">
            <button
              v-for="month in months"
              :key="month.value"
              @click="selectMonth(month.value)"
              class="py-2 rounded-xl text-sm font-bold transition-all capitalize"
              :class="[
                selectedMonth === month.value &&
                selectedYear === currentYearView
                  ? 'bg-[#6366f1] text-white shadow-lg shadow-indigo-500/30'
                  : 'text-gray-500 hover:bg-white/5 hover:text-white',
              ]"
            >
              {{ month.label }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <div
      class="flex-1 flex flex-col bg-[#18181b] p-8 rounded-4xl border border-white/5 shadow-2xl relative overflow-hidden"
    >
      <div
        v-if="loading && !isFirstLoad"
        class="absolute inset-0 bg-[#18181b]/80 backdrop-blur-sm z-10 flex items-center justify-center"
      >
        <div
          class="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-[#6366f1]"
        ></div>
      </div>

      <h3 class="text-white font-bold text-lg mb-10 tracking-tight">
        Despesas por categoria
      </h3>

      <div class="flex-1 w-full min-h-62.5">
        <apexchart
          v-if="chartSeries[0].data.length > 0"
          type="bar"
          height="100%"
          :options="chartOptions"
          :series="chartSeries"
        />
        <div
          v-else
          class="h-full flex items-center justify-center text-gray-500 font-medium"
        >
          Nenhuma despesa neste período.
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import {
  dashboardService,
  type DashboardResponse,
} from "../services/dashboardService";
import VueApexCharts from "vue3-apexcharts";
import type { ApexOptions } from "apexcharts";

const apexchart = VueApexCharts;

// Controle de Estado
const loading = ref(true);
const isFirstLoad = ref(true);
const dashboardData = ref<DashboardResponse>({} as DashboardResponse);

// Estado do DatePicker
const showDatePicker = ref(false);
const selectedMonth = ref(4);
const selectedYear = ref(2026);
const currentYearView = ref(2026);

const months = [
  { value: 1, label: "jan" },
  { value: 2, label: "fev" },
  { value: 3, label: "mar" },
  { value: 4, label: "abr" },
  { value: 5, label: "mai" },
  { value: 6, label: "jun" },
  { value: 7, label: "jul" },
  { value: 8, label: "ago" },
  { value: 9, label: "set" },
  { value: 10, label: "out" },
  { value: 11, label: "nov" },
  { value: 12, label: "dez" },
];

const formattedPeriod = computed(() => {
  const monthLabel = months
    .find((m) => m.value === selectedMonth.value)
    ?.label.toUpperCase();
  return `${monthLabel} / ${selectedYear.value}`;
});

const toggleDatePicker = () => {
  showDatePicker.value = !showDatePicker.value;
  if (showDatePicker.value) {
    currentYearView.value = selectedYear.value;
  }
};

const selectMonth = async (monthValue: number) => {
  selectedMonth.value = monthValue;
  selectedYear.value = currentYearView.value;
  showDatePicker.value = false;
  await loadData();
};

const formatBRL = (value: number) => {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value || 0);
};

const chartSeries = computed(() => [
  {
    name: "Total",
    data: dashboardData.value.graficoDespesas?.map((d) => d.valor) || [],
  },
]);

const chartOptions = computed<ApexOptions>(() => ({
  chart: {
    type: "bar" as const,
    toolbar: { show: false },
    background: "transparent",
    fontFamily: "Inter, sans-serif",
  },
  colors: [
    "#38bdf8",
    "#818cf8",
    "#34d399",
    "#fb923c",
    "#c084fc",
    "#e879f9",
    "#2dd4bf",
    "#f87171",
  ],
  plotOptions: {
    bar: {
      borderRadius: 6,
      columnWidth: "40%",
      distributed: true,
      dataLabels: { position: "top" },
    },
  },
  dataLabels: {
    enabled: true,
    formatter: (val: number) => formatBRL(val).replace(",00", ""),
    offsetY: -30,
    style: { fontSize: "12px", fontWeight: "bold", colors: ["#fff"] },
  },
  xaxis: {
    categories:
      dashboardData.value.graficoDespesas?.map((d) =>
        d.categoria.toUpperCase(),
      ) || [],
    labels: { style: { colors: "#fff", fontSize: "11px", fontWeight: "700" } },
    axisBorder: { show: false },
    axisTicks: { show: false },
  },
  yaxis: { show: false },
  grid: { show: false },
  theme: { mode: "dark" as const },
  legend: { show: false },
}));

const loadData = async () => {
  loading.value = true;
  try {
    dashboardData.value = await dashboardService.getResumo(
      selectedMonth.value,
      selectedYear.value,
    );
  } catch (error) {
    console.error("Erro no dashboard:", error);
  } finally {
    loading.value = false;
    isFirstLoad.value = false;
  }
};

onMounted(() => {
  loadData();
});
</script>

<style scoped>
@reference "tailwindcss";

.kpi-card {
  @apply bg-[#18181b] p-5 rounded-2xl border border-white/5 flex flex-col justify-center;
}
.label {
  @apply text-gray-500 text-[10px] font-black tracking-[0.15em] mb-1;
}
.value {
  @apply text-2xl font-black tracking-tight;
}
</style>
