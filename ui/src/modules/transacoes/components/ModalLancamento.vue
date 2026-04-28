<template>
  <div
    v-if="show"
    class="fixed inset-0 z-50 flex items-center justify-center p-4"
  >
    <div
      class="absolute inset-0 bg-black/60 backdrop-blur-sm"
      @click="$emit('close')"
    ></div>

    <div
      class="relative bg-[#111114] w-full max-w-lg rounded-3xl border border-white/5 shadow-2xl p-8 animate-in zoom-in-95 duration-200"
    >
      <div class="flex items-center justify-between mb-8">
        <h2 class="text-white text-[17px] font-black tracking-tight uppercase">
          {{ mode === "add" ? "Novo Lançamento" : "Editar Lançamento" }}
        </h2>
        <button
          @click="$emit('close')"
          class="text-gray-500 hover:text-white transition-colors p-1.5 hover:bg-white/5 rounded-full"
        >
          <X :size="18" />
        </button>
      </div>

      <form class="space-y-5" @submit.prevent="handleSubmit">
        <div class="space-y-1">
          <div
            class="grid grid-cols-2 gap-3 p-1.5 bg-[#18181b] rounded-2xl border border-white/5"
          >
            <button
              type="button"
              @click="setTipo('R')"
              :class="
                formData.tipo === 'R'
                  ? 'bg-emerald-500/20 text-emerald-400 border-emerald-500/10 shadow-lg'
                  : 'text-gray-600 border-transparent'
              "
              class="flex items-center justify-center gap-2.5 py-3.5 rounded-xl font-black uppercase text-[16px] tracking-widest transition-all border"
            >
              <TrendingUp :size="18" /> Receita
            </button>
            <button
              type="button"
              @click="setTipo('D')"
              :class="
                formData.tipo === 'D'
                  ? 'bg-red-500/20 text-red-500 border-red-500/10 shadow-lg'
                  : 'text-gray-600 border-transparent'
              "
              class="flex items-center justify-center gap-2.5 py-3.5 rounded-xl font-black uppercase text-[16px] tracking-widest transition-all border"
            >
              <TrendingDown :size="18" /> Despesa
            </button>
          </div>
        </div>

        <div class="relative">
          <label
            class="block text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1"
            >Categoria</label
          >
          <button
            type="button"
            @click="isDropdownOpen = !isDropdownOpen"
            class="w-full bg-[#18181b] border rounded-2xl px-4 h-13 flex justify-between items-center text-left transition-all focus:outline-none"
            :class="[
              isDropdownOpen ? 'border-[#6366f1]' : 'border-white/5',
              errors.categoriaId ? 'border-red-500/50' : '',
            ]"
          >
            <div class="flex items-center gap-3 truncate text-[13px]">
              <Tag :size="18" class="text-gray-600 shrink-0" />
              <span
                :class="
                  selectedCategoriaNome
                    ? 'text-white font-bold'
                    : 'text-gray-700'
                "
                class="truncate"
              >
                {{ selectedCategoriaNome || "Selecione uma opção..." }}
              </span>
            </div>
            <ChevronDown
              :size="16"
              class="text-gray-600 transition-transform duration-200"
              :class="{ 'rotate-180': isDropdownOpen }"
            />
          </button>
          <p
            v-if="errors.categoriaId"
            class="text-[13px] text-red-400 font-bold mt-1 ml-1 uppercase tracking-wider"
          >
            {{ errors.categoriaId }}
          </p>

          <div
            v-if="isDropdownOpen"
            class="absolute z-50 top-[calc(100%+8px)] left-0 w-full bg-[#1a1a1f] border border-white/10 rounded-2xl shadow-2xl overflow-hidden flex flex-col animate-in fade-in slide-in-from-top-2"
          >
            <div class="p-3 border-b border-white/5 bg-black/20">
              <div
                class="relative flex items-center gap-2 bg-black px-3 py-2 rounded-lg border border-white/5 focus-within:border-[#6366f1]/50"
              >
                <Search :size="14" class="text-gray-700" />
                <input
                  v-model="query"
                  type="text"
                  placeholder="Pesquisar..."
                  class="w-full bg-transparent text-[13px] text-white outline-none"
                  @click.stop
                />
              </div>
            </div>
            <div class="max-h-64 overflow-y-auto custom-scroll p-1">
              <div
                v-if="categoriasFiltradas.length === 0"
                class="p-4 text-center text-gray-600 text-[13px] font-black uppercase"
              >
                Nada encontrado
              </div>
              <button
                v-for="cat in categoriasFiltradas"
                :key="cat.id"
                type="button"
                @click="selecionarCategoria(cat.id, cat.nome)"
                class="w-full text-left px-4 py-3 flex items-center gap-3 transition-colors rounded-xl group"
                :class="[
                  cat.isSub
                    ? 'pl-10 text-gray-400 font-medium'
                    : 'font-black text-indigo-400 bg-white/2 mt-1 first:mt-0 uppercase text-[11px] tracking-widest pointer-events-none',
                  formData.categoriaId === cat.id
                    ? 'bg-[#6366f1] text-white'
                    : 'hover:bg-white/5',
                ]"
              >
                <component
                  :is="getLucideIcon(cat.icone)"
                  :size="cat.isSub ? 14 : 12"
                />
                <span class="truncate text-[13px]">{{ cat.nome }}</span>
                <Check
                  v-if="formData.categoriaId === cat.id"
                  :size="14"
                  class="ml-auto"
                />
              </button>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label
              class="block text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1"
              >Data</label
            >
            <div
              class="modal-input-group h-13"
              :class="errors.data ? 'border-red-500/50' : ''"
            >
              <Calendar :size="18" class="text-gray-600" />
              <input type="date" v-model="formData.data" class="modal-input" />
            </div>
            <p
              v-if="errors.data"
              class="text-[13px] text-red-400 font-bold mt-1 ml-1 uppercase tracking-wider"
            >
              {{ errors.data }}
            </p>
          </div>

          <div>
            <label
              class="block text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1"
              >Valor</label
            >
            <div
              class="modal-input-group h-13"
              :class="errors.valor ? 'border-red-500/50' : ''"
            >
              <input
                type="text"
                v-model="formData.valor"
                @input="handleValorInput"
                class="modal-input text-white font-bold"
                placeholder="0,00"
              />
            </div>
            <p
              v-if="errors.valor"
              class="text-[13px] text-red-400 font-bold mt-1 ml-1 uppercase tracking-wider"
            >
              {{ errors.valor }}
            </p>
          </div>
        </div>

        <div>
          <label
            class="block text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1"
            >Descrição Opcional</label
          >
          <div class="modal-input-group h-13">
            <input
              type="text"
              v-model="formData.descricao"
              class="modal-input"
              placeholder="Ex: Mercado, Aluguel..."
            />
          </div>
        </div>

        <div class="flex gap-4 pt-6">
          <button
            type="button"
            @click="$emit('close')"
            class="flex-1 py-4 text-gray-500 font-bold hover:text-white uppercase text-[13px] tracking-widest transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="flex-2 bg-[#6366f1] hover:bg-[#4f46e5] text-white py-4 rounded-xl font-black shadow-lg shadow-indigo-500/20 active:scale-95 uppercase text-[13px] tracking-widest transition-all"
          >
            {{ mode === "add" ? "Confirmar" : "Salvar" }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, reactive } from "vue";
import {
  X,
  TrendingUp,
  TrendingDown,
  Calendar,
  Tag,
  ChevronDown,
  Search,
  Check,
} from "lucide-vue-next";
import { getLucideIcon } from "@/shared/utils/iconMap";
import type { Categoria } from "@/modules/categorias/types";

const props = defineProps<{
  show: boolean;
  mode: "add" | "edit";
  transactionData?: any;
  categorias: Categoria[];
}>();

const emit = defineEmits(["close", "submit"]);

const isDropdownOpen = ref(false);
const query = ref("");
const selectedCategoriaNome = ref("");
const errors = reactive({ categoriaId: "", data: "", valor: "" });

const formData = ref({
  id: "",
  tipo: "D" as "R" | "D",
  data: "",
  descricao: "",
  valor: "",
  categoriaId: "",
});

const clearErrors = () => {
  errors.categoriaId = "";
  errors.data = "";
  errors.valor = "";
};

const findName = (id: string) => {
  if (!id) return "";
  const tid = id.toLowerCase();
  for (const c of props.categorias) {
    if (c.id.toLowerCase() === tid) return c.nome;
    const s = c.subcategorias?.find((sub) => sub.id.toLowerCase() === tid);
    if (s) return s.nome;
  }
  return "";
};

const sync = () => {
  clearErrors();
  if (props.mode === "edit" && props.transactionData) {
    const d = props.transactionData;
    formData.value = {
      id: d.id,
      tipo: d.tipo,
      data: d.data.substring(0, 10),
      descricao: d.descricao || "",
      valor: formatCurrency(d.valor),
      categoriaId: d.categoriaId,
    };
    selectedCategoriaNome.value = findName(d.categoriaId);
  } else {
    formData.value = {
      id: "",
      tipo: "D",
      data: new Date().toISOString().substring(0, 10),
      descricao: "",
      valor: "",
      categoriaId: "",
    };
    selectedCategoriaNome.value = "";
  }
};

watch(
  () => props.show,
  (val) => {
    if (val) sync();
  },
  { immediate: true },
);

const categoriasFiltradas = computed(() => {
  const base = props.categorias.filter((c) => c.tipo === formData.value.tipo);
  const q = query.value.toLowerCase();
  const res: any[] = [];
  base.forEach((cat) => {
    const mPai = cat.nome.toLowerCase().includes(q);
    const mSubs = (cat.subcategorias || []).filter((s) =>
      s.nome.toLowerCase().includes(q),
    );
    if (formData.value.tipo === "D") {
      if (mPai || mSubs.length > 0) {
        res.push({
          id: cat.id,
          nome: cat.nome,
          icone: cat.icone,
          isSub: false,
        });
        const filhas = mPai ? cat.subcategorias || [] : mSubs;
        filhas.forEach((s) =>
          res.push({ id: s.id, nome: s.nome, icone: s.icone, isSub: true }),
        );
      }
    } else if (mPai)
      res.push({ id: cat.id, nome: cat.nome, icone: cat.icone, isSub: false });
  });
  return res;
});

const selecionarCategoria = (id: string, nome: string) => {
  formData.value.categoriaId = id;
  selectedCategoriaNome.value = nome;
  isDropdownOpen.value = false;
  errors.categoriaId = "";
};

const setTipo = (t: "R" | "D") => {
  formData.value.tipo = t;
  formData.value.categoriaId = "";
  selectedCategoriaNome.value = "";
};

const formatCurrency = (v: any) => {
  const n =
    typeof v === "number"
      ? v
      : parseFloat(v?.toString().replace(/\D/g, "") || "0") / 100;
  return new Intl.NumberFormat("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(n);
};

const handleValorInput = (e: any) => {
  formData.value.valor = formatCurrency(e.target.value);
  errors.valor = "";
};

const validate = () => {
  let isValid = true;
  if (!formData.value.categoriaId) {
    errors.categoriaId = "Obrigatório";
    isValid = false;
  }
  if (!formData.value.data) {
    errors.data = "Obrigatório";
    isValid = false;
  }
  if (!formData.value.valor || formData.value.valor === "0,00") {
    errors.valor = "Obrigatório";
    isValid = false;
  }
  return isValid;
};

const handleSubmit = () => {
  if (!validate()) return;
  const val = parseFloat(
    formData.value.valor.replace(/\./g, "").replace(",", "."),
  );
  emit("submit", { ...formData.value, valor: val });
};
</script>

<style scoped>
@reference "tailwindcss";

.modal-input-group {
  @apply bg-[#18181b] px-4 rounded-2xl border border-white/5 flex items-center gap-3 transition-all focus-within:border-[#6366f1]/50;
}

.modal-input {
  @apply flex-1 bg-transparent text-[13px] text-gray-200 outline-none placeholder:text-gray-700 h-full;
}

input[type="date"]::-webkit-calendar-picker-indicator {
  filter: invert(1);
  cursor: pointer;
  opacity: 0.5;
}

.custom-scroll::-webkit-scrollbar {
  width: 4px;
}
.custom-scroll::-webkit-scrollbar-thumb {
  background: #333;
  border-radius: 10px;
}
</style>
