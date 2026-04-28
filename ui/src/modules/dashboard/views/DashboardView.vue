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
      <div class="kpi-card border-l-4 border-l-teal-500/50">
        <p class="label">SALDO INICIAL</p>
        <p class="value text-teal-400">
          {{ formatBRL(dashboardData.saldoInicial) }}
        </p>
      </div>

      <div class="kpi-card border-l-4 border-l-emerald-500/50">
        <p class="label">RECEITAS</p>
        <p class="value text-emerald-400">
          {{ formatBRL(dashboardData.totalReceitas) }}
        </p>
      </div>

      <div class="kpi-card border-l-4 border-l-red-500/50">
        <p class="label">DESPESAS</p>
        <p class="value text-red-500">
          {{ formatBRL(dashboardData.totalDespesas) }}
        </p>
      </div>

      <div class="kpi-card border-l-4 border-l-indigo-500/50">
        <p class="label">SALDO ATUAL</p>
        <p class="value text-indigo-400">
          {{ formatBRL(dashboardData.saldoAtual) }}
        </p>
      </div>

      <div class="relative">
        <div
          @click="toggleDatePicker"
          class="bg-[#18181b] p-5 rounded-2xl border border-white/5 flex flex-col justify-center items-center cursor-pointer hover:bg-white/10 transition-all h-full"
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
              class="text-gray-400 hover:text-white px-2 py-1 rounded hover:bg-white/5"
            >
              ❮
            </button>
            <span class="text-white font-bold">{{ currentYearView }}</span>
            <button
              @click="currentYearView++"
              class="text-gray-400 hover:text-white px-2 py-1 rounded hover:bg-white/5"
            >
              ❯
            </button>
          </div>

          <div class="grid grid-cols-3 gap-2">
            <button
              v-for="month in months"
              :key="month.value"
              @click="selectMonth(month.value)"
              class="py-2 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all"
              :class="[
                selectedMonth === month.value &&
                selectedYear === currentYearView
                  ? 'bg-[#6366f1] text-white shadow-lg'
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
        class="absolute inset-0 bg-[#18181b]/60 backdrop-blur-sm z-10 flex items-center justify-center"
      >
        <div
          class="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-[#6366f1]"
        ></div>
      </div>

      <div class="flex items-center justify-between mb-10">
        <h3
          class="text-white font-black tracking-tight uppercase text-xs opacity-50"
        >
          Distribuição de Despesas
        </h3>
        <div
          v-if="connection?.state === 'Connected'"
          class="flex items-center gap-2"
        >
          <span class="relative flex h-2 w-2">
            <span
              class="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"
            ></span>
            <span
              class="relative inline-flex rounded-full h-2 w-2 bg-emerald-500"
            ></span>
          </span>
          <span
            class="text-[8px] font-black text-emerald-500 uppercase tracking-widest"
            >Live</span
          >
        </div>
      </div>

      <div class="flex-1 w-full min-h-87.5">
        <apexchart
          v-if="chartSeries[0].data.length > 0"
          type="bar"
          height="100%"
          :options="chartOptions"
          :series="chartSeries"
        />
        <div
          v-else
          class="h-full flex flex-col items-center justify-center text-gray-600 gap-4"
        >
          <div class="p-4 bg-white/5 rounded-full">
            <BarChart3 :size="48" class="opacity-20" />
          </div>
          <p class="font-black uppercase text-[10px] tracking-[0.3em]">
            Nenhuma despesa registrada
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from "vue";
import {
  dashboardService,
  type DashboardResponse,
} from "../services/dashboardService";
import { useSignalR } from "@/shared/composables/useSignalR";
import { BarChart3 } from "lucide-vue-next";
import VueApexCharts from "vue3-apexcharts";
import type { ApexOptions } from "apexcharts";

const apexchart = VueApexCharts;
const { connection, startConnection, stopConnection } = useSignalR();

// Controle de Estado
const loading = ref(true);
const isFirstLoad = ref(true);
const dashboardData = ref<DashboardResponse>({
  saldoInicial: 0,
  totalReceitas: 0,
  totalDespesas: 0,
  saldoAtual: 0,
  graficoDespesas: [],
});

// Estado do DatePicker (Iniciando em Abril/2026 conforme seu projeto)
const selectedMonth = ref(4);
const selectedYear = ref(2026);
const currentYearView = ref(2026);
const showDatePicker = ref(false);

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

// Lógica do Gráfico
const chartSeries = computed(() => [
  {
    name: "Total",
    data: dashboardData.value.graficoDespesas?.map((d) => d.valor) || [],
  },
]);

const chartOptions = computed<ApexOptions>(() => ({
  chart: {
    type: "bar",
    toolbar: { show: false },
    fontFamily: "Inter, sans-serif",
  },
  colors: [
    "#6366f1",
    "#10b981",
    "#f59e0b",
    "#ef4444",
    "#8b5cf6",
    "#ec4899",
    "#06b6d4",
  ],
  plotOptions: {
    bar: {
      borderRadius: 8,
      columnWidth: "45%",
      distributed: true,
      dataLabels: { position: "top" },
    },
  },
  dataLabels: {
    enabled: true,
    formatter: (val: number) => formatBRL(val).replace(",00", ""),
    offsetY: -25,
    style: { fontSize: "10px", fontWeight: "900", colors: ["#fff"] },
  },
  xaxis: {
    categories:
      dashboardData.value.graficoDespesas?.map((d) =>
        d.categoria.toUpperCase(),
      ) || [],
    labels: {
      style: { colors: "#64748b", fontSize: "10px", fontWeight: "800" },
    },
    axisBorder: { show: false },
    axisTicks: { show: false },
  },
  yaxis: { show: false },
  grid: { show: false },
  theme: { mode: "dark" },
  legend: { show: false },
  tooltip: {
    theme: "dark",
    y: { formatter: (val) => formatBRL(val) },
  },
}));

// Ações
const loadData = async () => {
  loading.value = true;
  try {
    dashboardData.value = await dashboardService.getResumo(
      selectedMonth.value,
      selectedYear.value,
    );
  } catch (error) {
    console.error("Dashboard Error:", error);
  } finally {
    loading.value = false;
    isFirstLoad.value = false;
  }
};

const selectMonth = (monthValue: number) => {
  selectedMonth.value = monthValue;
  selectedYear.value = currentYearView.value;
  showDatePicker.value = false;
  loadData();
};

const toggleDatePicker = () => {
  showDatePicker.value = !showDatePicker.value;
  if (showDatePicker.value) currentYearView.value = selectedYear.value;
};

const formatBRL = (value: number) => {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value || 0);
};

// Ciclo de Vida & SignalR
onMounted(async () => {
  await loadData();

  // Inicia conexão SignalR
  const token = localStorage.getItem("token");
  if (token) {
    await startConnection(token);
    // Escuta o evento enviado pelo seu Backend (NotificarAtualizacaoDashboard)
    connection.value?.on("AtualizarDashboard", () => {
      loadData();
    });
  }
});

onUnmounted(() => {
  stopConnection();
});
</script>

<style scoped>
@reference "tailwindcss";

.kpi-card {
  @apply bg-[#18181b] p-6 rounded-3xl border border-white/5 flex flex-col justify-center transition-all hover:bg-white/2;
}
.label {
  @apply text-gray-500 text-[10px] font-black tracking-[0.2em] mb-2 uppercase;
}
.value {
  @apply text-2xl font-black tracking-tighter;
}
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
</style>
