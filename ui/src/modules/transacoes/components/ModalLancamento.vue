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
        <h2 class="text-white text-xl font-black tracking-tight">
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
            class="grid grid-cols-2 gap-3 p-1.5 bg-[#18181b] rounded-2xl border transition-all"
            :class="errors.tipo ? 'border-red-500/50' : 'border-white/5'"
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
          <p
            v-if="errors.tipo"
            class="text-[10px] text-red-400 font-bold ml-2 uppercase tracking-wider"
          >
            {{ errors.tipo }}
          </p>
        </div>

        <div class="relative">
          <label
            class="text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1 block"
            >Categoria</label
          >

          <Combobox v-model="formData.categoriaId">
            <div class="relative">
              <ComboboxButton
                class="w-full outline-none text-left"
                @click="errors.categoriaId = ''"
              >
                <div
                  class="modal-input-group flex items-center h-14.5 transition-all"
                  :class="{
                    'border-[#6366f1]/50 bg-black shadow-[0_0_15px_rgba(99,102,241,0.1)]':
                      formData.categoriaId,
                    'border-red-500/50': errors.categoriaId,
                  }"
                >
                  <Tag :size="18" class="text-gray-600 shrink-0" />
                  <span
                    v-if="formData.categoriaId"
                    class="flex-1 px-1 text-[13px] text-white font-medium truncate"
                  >
                    {{ findCategoryName(formData.categoriaId) }}
                  </span>
                  <span v-else class="flex-1 px-1 text-[13px] text-gray-700">
                    Selecione uma opção...
                  </span>
                  <ChevronDown :size="16" class="text-gray-600 shrink-0" />
                </div>
              </ComboboxButton>
              <p
                v-if="errors.categoriaId"
                class="text-[10px] text-red-400 font-bold mt-1 ml-1 uppercase tracking-wider"
              >
                {{ errors.categoriaId }}
              </p>

              <transition
                enter-active-class="transition duration-100 ease-out"
                enter-from-class="transform scale-95 opacity-0"
                enter-to-class="transform scale-100 opacity-100"
                leave-active-class="transition duration-75 ease-in"
                leave-from-class="transform scale-100 opacity-100"
                leave-to-class="transform scale-95 opacity-0"
                @after-enter="focusSearch"
              >
                <ComboboxOptions
                  class="absolute z-50 top-[calc(100%+8px)] left-0 w-full bg-[#18181b] border border-white/10 rounded-2xl shadow-2xl overflow-hidden"
                >
                  <div
                    class="p-3 border-b border-white/5 bg-black/20"
                    @click.stop
                    @mousedown.stop
                  >
                    <div
                      class="flex items-center gap-2 bg-black px-3 py-2.5 rounded-lg border border-white/5 focus-within:border-[#6366f1]/50"
                    >
                      <Search :size="14" class="text-gray-700" />
                      <input
                        ref="searchInput"
                        v-model="query"
                        type="text"
                        class="bg-transparent text-[13px] text-white outline-none w-full placeholder:text-gray-800"
                        placeholder="Pesquisar categoria..."
                        @keydown.stop
                      />
                    </div>
                  </div>

                  <div class="max-h-60 overflow-y-auto custom-scroll">
                    <div
                      v-if="filteredCategories.length === 0"
                      class="p-4 text-center text-[10px] text-gray-600 font-black uppercase"
                    >
                      Nenhuma categoria encontrada
                    </div>

                    <template v-if="formData.tipo === 'D'">
                      <div
                        v-for="pai in filteredCategories"
                        :key="pai.id"
                        class="border-b border-white/5 last:border-0"
                      >
                        <div
                          class="px-4 py-3 text-[13px] font-black text-indigo-400/90 bg-white/3 uppercase tracking-[0.15em] flex items-center gap-2.5"
                        >
                          <component
                            :is="getLucideIcon(pai.icone)"
                            :size="13"
                            class="opacity-80"
                          />
                          {{ pai.nome }}
                        </div>

                        <ComboboxOption
                          v-for="sub in pai.subcategorias"
                          :key="sub.id"
                          :value="sub.id"
                          v-slot="{ active, selected }"
                        >
                          <li
                            :class="[
                              active
                                ? 'bg-[#6366f1] text-white'
                                : 'text-gray-400',
                            ]"
                            class="px-10 py-3.5 text-[13px] font-bold flex items-center justify-between cursor-pointer transition-colors"
                          >
                            <span class="flex items-center gap-2">
                              <component
                                :is="getLucideIcon(sub.icone)"
                                :size="14"
                                class="opacity-50"
                              />
                              {{ sub.nome }}
                            </span>
                            <Check v-if="selected" :size="14" />
                          </li>
                        </ComboboxOption>
                      </div>
                    </template>

                    <template v-else>
                      <ComboboxOption
                        v-for="cat in filteredCategories"
                        :key="cat.id"
                        :value="cat.id"
                        v-slot="{ active, selected }"
                      >
                        <li
                          :class="[
                            active
                              ? 'bg-[#6366f1] text-white'
                              : 'text-gray-400',
                          ]"
                          class="px-4 py-3.5 text-[13px] font-bold flex items-center justify-between cursor-pointer transition-colors"
                        >
                          <span class="flex items-center gap-3">
                            <component
                              :is="getLucideIcon(cat.icone)"
                              :size="16"
                            />
                            {{ cat.nome }}
                          </span>
                          <Check v-if="selected" :size="14" />
                        </li>
                      </ComboboxOption>
                    </template>
                  </div>
                </ComboboxOptions>
              </transition>
            </div>
          </Combobox>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="flex flex-col gap-1">
            <label
              class="text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1"
              >Data</label
            >
            <div
              class="modal-input-group h-14.5"
              :class="errors.data ? 'border-red-500/50' : ''"
            >
              <Calendar :size="18" class="text-gray-600" />
              <input
                type="date"
                v-model="formData.data"
                @input="errors.data = ''"
                class="modal-input"
              />
            </div>
            <p
              v-if="errors.data"
              class="text-[10px] text-red-400 font-bold ml-1 uppercase tracking-wider"
            >
              {{ errors.data }}
            </p>
          </div>

          <div class="flex flex-col gap-1">
            <label
              class="text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1"
              >Valor</label
            >
            <div
              class="modal-input-group h-14.5"
              :class="errors.valor ? 'border-red-500/50' : ''"
            >
              <input
                type="text"
                v-model="formData.valor"
                @input="handleValorInput"
                placeholder="0,00"
                class="modal-input text-white font-bold"
              />
            </div>
            <p
              v-if="errors.valor"
              class="text-[10px] text-red-400 font-bold ml-1 uppercase tracking-wider"
            >
              {{ errors.valor }}
            </p>
          </div>
        </div>

        <div class="flex flex-col gap-1">
          <label
            class="text-[13px] font-black text-gray-600 uppercase tracking-widest ml-1"
            >Descrição Opcional</label
          >
          <div class="modal-input-group h-14.5">
            <input
              type="text"
              v-model="formData.descricao"
              placeholder="Ex: Mercado, Aluguer, Freelance..."
              class="modal-input"
            />
          </div>
        </div>

        <div class="flex gap-4 pt-6">
          <button
            type="button"
            @click="$emit('close')"
            class="flex-1 py-4 text-gray-500 font-bold hover:text-white transition-colors uppercase text-[13px] tracking-widest"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="flex-2 bg-[#6366f1] hover:bg-[#4f46e5] text-white py-4 rounded-xl font-bold transition-all shadow-lg shadow-indigo-500/20 active:scale-95 uppercase text-[13px] tracking-widest"
          >
            Confirmar Lançamento
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick, reactive } from "vue";
import {
  Combobox,
  ComboboxButton,
  ComboboxOptions,
  ComboboxOption,
} from "@headlessui/vue";
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
import {
  categoriaService,
  type CategoriaResponse,
} from "../services/categoriaService";
import { getLucideIcon } from "@/shared/utils/iconMap";

const props = defineProps<{
  show: boolean;
  mode: "add" | "edit";
  transactionData?: any;
}>();

const emit = defineEmits(["close", "submit"]);

const searchInput = ref<HTMLInputElement | null>(null);
const allCategories = ref<CategoriaResponse[]>([]);
const query = ref("");

const errors = reactive({
  tipo: "",
  categoriaId: "",
  data: "",
  valor: "",
});

const getInitialState = () => ({
  id: "",
  tipo: "D" as "R" | "D",
  data: new Date().toISOString().substring(0, 10),
  descricao: "",
  valor: "" as string,
  categoriaId: "",
});

const formData = ref(getInitialState());

const formatCurrency = (value: number | string) => {
  const amount =
    typeof value === "number"
      ? value
      : parseFloat(value.replace(/\D/g, "")) / 100;
  if (isNaN(amount)) return "";
  return new Intl.NumberFormat("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
};

const handleValorInput = (e: Event) => {
  const input = e.target as HTMLInputElement;
  let value = input.value.replace(/\D/g, "");

  if (!value) {
    formData.value.valor = "";
    return;
  }

  formData.value.valor = formatCurrency(value);
  errors.valor = "";
};

const setTipo = (t: "R" | "D") => {
  formData.value.tipo = t;
  errors.tipo = "";
};

const clearErrors = () => {
  errors.tipo = "";
  errors.categoriaId = "";
  errors.data = "";
  errors.valor = "";
};

watch(
  () => props.show,
  (isShowing) => {
    if (isShowing) {
      clearErrors();
      if (props.mode === "edit" && props.transactionData) {
        const data = { ...props.transactionData };
        data.valor = formatCurrency(data.valor);
        formData.value = data;
      } else {
        formData.value = getInitialState();
      }
      query.value = "";
    }
  },
  { immediate: true },
);

const focusSearch = () => {
  nextTick(() => {
    searchInput.value?.focus();
  });
};

onMounted(async () => {
  try {
    allCategories.value = await categoriaService.getCategorias();
  } catch (e) {
    console.error("Erro ao carregar categorias no modal:", e);
  }
});

const filteredCategories = computed(() => {
  const typeFiltered = allCategories.value.filter(
    (cat) => cat.tipo === formData.value.tipo,
  );
  if (query.value === "") return typeFiltered;

  const searchTerm = query.value.toLowerCase();
  return typeFiltered.filter((cat) => {
    const matchPai = cat.nome.toLowerCase().includes(searchTerm);
    const matchFilha = cat.subcategorias?.some((sub) =>
      sub.nome.toLowerCase().includes(searchTerm),
    );
    return matchPai || matchFilha;
  });
});

const findCategoryName = (id: string) => {
  if (!id) return "";
  for (const cat of allCategories.value) {
    if (cat.id === id) return cat.nome;
    const sub = cat.subcategorias?.find((s) => s.id === id);
    if (sub) return sub.nome;
  }
  return "Selecione...";
};

watch(
  () => formData.value.tipo,
  (newVal, oldVal) => {
    if (props.show && props.mode === "add" && newVal !== oldVal) {
      formData.value.categoriaId = "";
      query.value = "";
    }
  },
);

const validate = () => {
  clearErrors();
  let isValid = true;

  if (!formData.value.tipo) {
    errors.tipo = "Selecione o tipo";
    isValid = false;
  }
  if (!formData.value.categoriaId) {
    errors.categoriaId = "Selecione a categoria";
    isValid = false;
  }
  if (!formData.value.data) {
    errors.data = "Informe a data";
    isValid = false;
  }

  const rawValue = formData.value.valor
    .toString()
    .replace(/\./g, "")
    .replace(",", ".");
  const valorNum = parseFloat(rawValue);

  if (!formData.value.valor || isNaN(valorNum) || valorNum <= 0) {
    errors.valor = "Valor inválido";
    isValid = false;
  }

  return isValid;
};

const handleSubmit = () => {
  if (!validate()) return;

  const cleanValor =
    typeof formData.value.valor === "string"
      ? parseFloat(formData.value.valor.replace(/\./g, "").replace(",", "."))
      : formData.value.valor;

  emit("submit", {
    ...formData.value,
    valor: cleanValor,
  });
};
</script>

<style scoped>
@reference "tailwindcss";

.modal-input-group {
  @apply bg-[#18181b] p-4 rounded-2xl border border-white/5 flex items-center gap-3 transition-all focus-within:border-[#6366f1] focus-within:bg-black;
}
.modal-input {
  /* VALORES DOS INPUTS PADRONIZADOS EM 13px */
  @apply flex-1 bg-transparent text-[13px] text-gray-200 outline-none placeholder:text-gray-700;
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
