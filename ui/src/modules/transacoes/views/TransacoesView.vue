<template>
  <div
    class="flex flex-col flex-1 space-y-6 animate-in fade-in duration-500 relative"
  >
    <div
      class="flex items-center justify-between gap-4 z-10 sticky top-0 bg-black pt-6 pb-2 -mt-6"
    >
      <button
        @click="openAddModal"
        class="bg-[#6366f1] hover:bg-[#4f46e5] text-white px-6 py-3 rounded-xl font-bold flex items-center gap-2.5 transition-all shadow-lg shadow-indigo-500/20 active:scale-95 shrink-0"
      >
        <PlusCircle :size="18" />
        Novo Lançamento
      </button>

      <div
        class="flex-1 flex bg-[#18181b] p-1.5 rounded-2xl border border-white/5 overflow-x-auto no-scrollbar"
      >
        <button
          v-for="m in months"
          :key="m.value"
          @click="changeMonth(m.value)"
          :class="[
            selectedMonth === m.value
              ? 'bg-[#6366f1] text-white shadow-lg scale-105'
              : 'text-gray-500 hover:text-gray-300',
          ]"
          class="flex-1 px-5 py-3 rounded-xl text-xs font-black uppercase tracking-widest transition-all whitespace-nowrap"
        >
          {{ m.label }}
        </button>
      </div>

      <div class="relative shrink-0">
        <div
          @click="showYearPicker = !showYearPicker"
          class="bg-[#18181b] px-6 py-3 rounded-xl border border-white/5 text-white font-black text-sm cursor-pointer hover:bg-white/5 transition-all flex items-center gap-3"
        >
          <CalendarDays :size="16" class="text-indigo-400" />
          <span>{{ selectedYear }}</span>
          <span
            class="text-[10px] transition-transform duration-300"
            :class="{ 'rotate-180': showYearPicker }"
            >▼</span
          >
        </div>
        <div
          v-if="showYearPicker"
          @click="showYearPicker = false"
          class="fixed inset-0 z-40"
        ></div>
        <div
          v-if="showYearPicker"
          class="absolute top-[calc(100%+8px)] right-0 w-32 bg-[#18181b] border border-white/5 rounded-2xl shadow-2xl p-2 z-50 animate-in slide-in-from-top-2 duration-200"
        >
          <button
            v-for="year in availableYears"
            :key="year"
            @click="changeYear(year)"
            :class="
              selectedYear === year
                ? 'bg-[#6366f1] text-white'
                : 'text-gray-500 hover:bg-white/5 hover:text-white'
            "
            class="w-full py-3 rounded-xl text-sm font-black transition-all mb-1 last:mb-0"
          >
            {{ year }}
          </button>
        </div>
      </div>
    </div>

    <div
      class="flex-1 bg-[#18181b] rounded-4xl border border-white/5 shadow-2xl flex flex-col overflow-hidden"
    >
      <div
        class="p-6 flex justify-between items-center border-b border-white/5 bg-white/1"
      >
        <div class="flex items-center gap-3">
          <select
            v-model="perPage"
            class="bg-black border border-white/10 rounded-lg text-xs p-2 text-white outline-none focus:border-[#6366f1]"
          >
            <option :value="5">5 registros</option>
            <option :value="10">10 registros</option>
            <option :value="20">20 registros</option>
          </select>
          <span
            class="text-[10px] text-gray-500 font-black uppercase tracking-widest"
            >por página</span
          >
        </div>

        <div class="relative w-80">
          <input
            v-model="search"
            type="text"
            placeholder="Buscar lançamentos..."
            class="w-full bg-black border border-white/10 rounded-xl py-2.5 px-4 pl-10 text-sm text-white focus:border-[#6366f1] outline-none transition-all placeholder:text-gray-700"
            @input="currentPage = 1"
          />
          <Search :size="16" class="absolute left-4 top-3.5 text-gray-600" />
        </div>
      </div>

      <div class="flex-1 overflow-y-auto relative custom-scroll">
        <div
          v-if="loading"
          class="absolute inset-0 bg-[#18181b]/50 backdrop-blur-sm z-20 flex items-center justify-center"
        >
          <div
            class="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-[#6366f1]"
          ></div>
        </div>

        <table class="w-full text-left border-collapse">
          <thead class="sticky top-0 bg-[#18181b] z-10 shadow-sm">
            <tr
              class="text-[10px] text-gray-500 font-black uppercase tracking-widest border-b border-white/5"
            >
              <th class="px-8 py-4">Data</th>
              <th class="px-8 py-4">Categoria</th>
              <th class="px-8 py-4">Subcategoria</th>
              <th class="px-8 py-4">Descrição</th>
              <th class="px-8 py-4 text-right">Valor</th>
              <th class="px-8 py-4 text-center">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-white/5">
            <tr
              v-for="t in paginatedData"
              :key="t.id"
              class="hover:bg-white/2 transition-colors"
            >
              <td class="px-8 py-5 text-sm font-medium text-gray-400">
                {{ formatDate(t.data) }}
              </td>

              <td class="px-8 py-5">
                <div class="flex items-center gap-2.5">
                  <component
                    :is="getLucideIcon(t.categoriaPaiIcone)"
                    class="text-indigo-400 opacity-80"
                    :size="18"
                  />
                  <span
                    class="text-xs font-black uppercase text-white tracking-tighter"
                    >{{ t.categoriaPaiNome }}</span
                  >
                </div>
              </td>

              <td class="px-8 py-5">
                <span
                  v-if="t.subcategoriaNome"
                  class="inline-flex items-center gap-1.5 text-[10px] font-bold text-gray-500 uppercase bg-white/5 px-2.5 py-1 rounded-md border border-white/5 whitespace-nowrap"
                >
                  <component
                    v-if="t.subcategoriaIcone"
                    :is="getLucideIcon(t.subcategoriaIcone)"
                    :size="12"
                    class="opacity-70"
                  />
                  {{ t.subcategoriaNome }}
                </span>
                <span v-else class="text-gray-700 text-sm font-bold">---</span>
              </td>

              <td class="px-8 py-5 text-sm text-gray-300 max-w-xs truncate">
                {{ t.descricao || "---" }}
              </td>

              <td
                class="px-8 py-5 text-right font-black text-sm"
                :class="t.tipo === 'R' ? 'text-emerald-400' : 'text-red-500'"
              >
                {{ t.tipo === "R" ? "+" : "-" }} {{ formatBRL(t.valor) }}
              </td>

              <td class="px-8 py-5 text-center">
                <div class="flex justify-center gap-3">
                  <button
                    @click="openEditModal(t)"
                    class="p-2 rounded-lg bg-white/5 border border-white/5 text-gray-500 hover:text-white hover:border-white/10 transition-colors"
                  >
                    <Pencil :size="16" />
                  </button>
                  <button
                    @click="confirmDelete(t.id)"
                    class="p-2 rounded-lg bg-white/5 border border-white/5 text-gray-500 hover:text-red-500 hover:border-red-500/20 hover:bg-red-500/5 transition-colors"
                  >
                    <Trash2 :size="16" />
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="!loading && filteredData.length === 0">
              <td
                colspan="6"
                class="px-8 py-20 text-center text-gray-600 text-xs font-black uppercase tracking-[0.2em]"
              >
                Nenhum lançamento encontrado neste período
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div
        class="p-4 bg-black/20 flex justify-between items-center px-8 border-t border-white/5"
      >
        <span
          class="text-[10px] text-gray-600 font-black uppercase tracking-widest"
          >{{ filteredData.length }} resultados</span
        >
        <div class="flex items-center gap-3">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="page-btn"
          >
            ANTERIOR
          </button>
          <div class="flex gap-1.5">
            <span
              v-for="p in totalPages"
              :key="p"
              @click="currentPage = p"
              :class="
                currentPage === p
                  ? 'bg-[#6366f1] text-white'
                  : 'text-gray-600 hover:text-gray-300 hover:bg-white/5'
              "
              class="w-7 h-7 flex items-center justify-center rounded-lg text-[10px] font-black cursor-pointer transition-all"
            >
              {{ p }}
            </span>
          </div>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="page-btn"
          >
            PRÓXIMO
          </button>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="summary-card border-l-gray-700">
        <p class="label">SALDO INICIAL</p>
        <p class="text-white text-xl font-black">
          {{ formatBRL(resumo.saldoInicial) }}
        </p>
      </div>
      <div class="summary-card border-l-emerald-500">
        <p class="label text-emerald-500">RECEITAS</p>
        <p class="text-emerald-400 text-xl font-black tracking-tighter">
          + {{ formatBRL(resumo.totalReceitas) }}
        </p>
      </div>
      <div class="summary-card border-l-red-500">
        <p class="label text-red-500">DESPESAS</p>
        <p class="text-red-500 text-xl font-black tracking-tighter">
          - {{ formatBRL(resumo.totalDespesas) }}
        </p>
      </div>
      <div class="summary-card border-l-indigo-500 bg-indigo-500/5">
        <p class="label text-indigo-400">SALDO FINAL</p>
        <p class="text-indigo-400 text-xl font-black tracking-tighter">
          {{ formatBRL(resumo.saldoAtual) }}
        </p>
      </div>
    </div>

    <ModalLancamento
      :show="modal.show"
      :mode="modal.mode"
      :transactionData="modal.data"
      :categorias="categoriasList"
      @close="closeModal"
      @submit="handleModalSubmit"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, reactive } from "vue";
import { transacaoService, type Transacao } from "../services/transacaoService";

// ⚡ IMPORT ATUALIZADO PARA O SERVIÇO UNIFICADO
import { categoriaService } from "@/modules/categorias/services/categoriaService";
import type { Categoria } from "@/modules/categorias/types";

import {
  PlusCircle,
  Pencil,
  Trash2,
  CalendarDays,
  Search,
} from "lucide-vue-next";
import ModalLancamento from "../components/ModalLancamento.vue";
import { getLucideIcon } from "@/shared/utils/iconMap";
import { useToast } from "@/shared/composables/useToast";
import { useNotification } from "@/shared/composables/useNotification";

const toast = useToast();
const notify = useNotification();

// Estado de Período
const now = new Date();
const currentYear = now.getFullYear();
const currentMonth = now.getMonth() + 1;

const loading = ref(true);
const selectedMonth = ref(currentMonth);
const selectedYear = ref(currentYear);
const showYearPicker = ref(false);
const availableYears = computed(() =>
  Array.from({ length: 5 }, (_, i) => currentYear - i),
);

// Estado da Tabela
const search = ref("");
const currentPage = ref(1);
const perPage = ref(10);
const allTransacoes = ref<Transacao[]>([]);
const categoriasList = ref<Categoria[]>([]); // Estado das categorias carregadas antecipadamente
const resumo = reactive({
  saldoInicial: 0,
  totalReceitas: 0,
  totalDespesas: 0,
  saldoAtual: 0,
});

// Estado do Modal
const modal = reactive({
  show: false,
  mode: "add" as "add" | "edit",
  data: null as any,
});

const months = [
  { value: 1, label: "Jan" },
  { value: 2, label: "Fev" },
  { value: 3, label: "Mar" },
  { value: 4, label: "Abr" },
  { value: 5, label: "Mai" },
  { value: 6, label: "Jun" },
  { value: 7, label: "Jul" },
  { value: 8, label: "Ago" },
  { value: 9, label: "Set" },
  { value: 10, label: "Out" },
  { value: 11, label: "Nov" },
  { value: 12, label: "Dez" },
];

// Lógica Computada da Tabela
const filteredData = computed(() => {
  if (!search.value) return allTransacoes.value;
  const s = search.value.toLowerCase();
  return allTransacoes.value.filter(
    (t) =>
      (t.descricao && t.descricao.toLowerCase().includes(s)) ||
      (t.subcategoriaNome && t.subcategoriaNome.toLowerCase().includes(s)) ||
      (t.categoriaPaiNome && t.categoriaPaiNome.toLowerCase().includes(s)),
  );
});

const totalPages = computed(
  () => Math.ceil(filteredData.value.length / perPage.value) || 1,
);

const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * perPage.value;
  return filteredData.value.slice(start, start + perPage.value);
});

// ⚡ CARREGAMENTO USANDO A NOVA FUNÇÃO UNIFICADA
const loadCategorias = async () => {
  try {
    categoriasList.value = await categoriaService.getCategorias();
  } catch (error) {
    console.error("Erro ao carregar categorias para o modal:", error);
  }
};

// Carregamento de Dados da API
const loadData = async () => {
  loading.value = true;
  try {
    const response = await transacaoService.getPorMes(
      selectedMonth.value,
      selectedYear.value,
    );
    resumo.saldoInicial = response.saldoInicial;
    resumo.totalReceitas = response.totalReceitas;
    resumo.totalDespesas = response.totalDespesas;
    resumo.saldoAtual = response.saldoAtual;
    allTransacoes.value = response.transacoes;
  } catch (error) {
    toast.error("Erro ao carregar as transações.");
    console.error("Erro ao carregar transações:", error);
  } finally {
    loading.value = false;
  }
};

const changeMonth = (m: number) => {
  selectedMonth.value = m;
  currentPage.value = 1;
  loadData();
};

const changeYear = (year: number) => {
  selectedYear.value = year;
  showYearPicker.value = false;
  currentPage.value = 1;
  loadData();
};

// Funções do Modal
const openAddModal = () => {
  modal.mode = "add";
  modal.data = null;
  modal.show = true;
};

const openEditModal = (transacao: Transacao) => {
  console.log(
    "📦 Dados recebidos para edição:",
    JSON.parse(JSON.stringify(transacao)),
  );

  modal.mode = "edit";

  // Criamos o objeto que vai para o modal
  modal.data = {
    ...transacao,
    data: transacao.data.substring(0, 10),
  };

  modal.show = true;
};

const closeModal = () => {
  modal.show = false;
};

const handleModalSubmit = async (payload: any) => {
  try {
    loading.value = true;
    if (modal.mode === "add") {
      await transacaoService.criar(payload);
      toast.success("Lançamento incluído com sucesso!");
    } else {
      await transacaoService.atualizar(payload.id, payload);
      toast.success("Lançamento atualizado com sucesso!");
    }
    closeModal();
    await loadData();
  } catch (error: any) {
    const msgErro =
      error.response?.data?.message ||
      "Ocorreu um erro ao processar o lançamento.";
    toast.error(msgErro);
    console.error("Erro ao salvar lançamento:", error);
  } finally {
    loading.value = false;
  }
};

// LÓGICA DE EXCLUSÃO
const confirmDelete = (id: string) => {
  notify.confirm(
    "Esta ação não pode ser desfeita. O saldo será recalculado imediatamente.",
    async () => {
      try {
        loading.value = true;
        await transacaoService.remover(id);
        toast.success("Lançamento excluído com sucesso!");
        await loadData();
      } catch (error: any) {
        const msgErro =
          error.response?.data?.message || "Erro ao excluir a transação.";
        toast.error(msgErro);
        console.error("Erro ao deletar:", error);
      } finally {
        loading.value = false;
      }
    },
    "Remover Lançamento?",
  );
};

// Formatadores
const formatBRL = (v?: number) =>
  new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" }).format(
    v || 0,
  );
const formatDate = (d: string) => {
  const date = new Date(d);
  return date.toLocaleDateString("pt-BR", { timeZone: "UTC" });
};

onMounted(() => {
  loadData();
  loadCategorias();
});
</script>

<style scoped>
@reference "tailwindcss";

.summary-card {
  @apply bg-[#18181b] p-5 rounded-2xl border border-white/5 flex flex-col items-start justify-center border-l-4 shadow-sm transition-all hover:border-white/10;
}
.label {
  @apply text-gray-500 text-[9px] font-black tracking-widest mb-1 uppercase;
}
.page-btn {
  @apply text-gray-500 hover:text-white text-[10px] font-black tracking-widest disabled:opacity-10 transition-all uppercase;
}
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
.custom-scroll::-webkit-scrollbar {
  width: 4px;
}
.custom-scroll::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scroll::-webkit-scrollbar-thumb {
  background: #333;
  border-radius: 10px;
}
</style>
