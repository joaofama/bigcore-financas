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
        <div
          class="grid grid-cols-2 gap-3 p-1.5 bg-[#18181b] rounded-2xl border border-white/5"
        >
          <button
            type="button"
            @click="formData.tipo = 'R'"
            :class="
              formData.tipo === 'R'
                ? 'bg-emerald-500/20 text-emerald-400 border-emerald-500/10 shadow-lg'
                : 'text-gray-600 border-transparent'
            "
            class="flex items-center justify-center gap-2.5 py-3.5 rounded-xl font-black uppercase text-xs tracking-[0.15em] transition-all border"
          >
            <TrendingUp :size="16" /> Receita
          </button>
          <button
            type="button"
            @click="formData.tipo = 'D'"
            :class="
              formData.tipo === 'D'
                ? 'bg-red-500/20 text-red-500 border-red-500/10 shadow-lg'
                : 'text-gray-600 border-transparent'
            "
            class="flex items-center justify-center gap-2.5 py-3.5 rounded-xl font-black uppercase text-xs tracking-[0.15em] transition-all border"
          >
            <TrendingDown :size="16" /> Despesa
          </button>
        </div>

        <div class="relative">
          <label
            class="text-[9px] font-black text-gray-600 uppercase tracking-widest ml-1 mb-1 block"
            >Categoria</label
          >

          <Combobox v-model="formData.categoriaId">
            <div class="relative">
              <ComboboxButton class="w-full outline-none text-left">
                <div
                  class="modal-input-group flex items-center h-14.5 transition-all"
                  :class="{
                    'border-[#6366f1]/50 bg-black': formData.categoriaId,
                  }"
                >
                  <Tag :size="18" class="text-gray-600 shrink-0" />

                  <span
                    v-if="formData.categoriaId"
                    class="flex-1 px-1 text-sm text-white font-medium truncate"
                  >
                    {{ findCategoryName(formData.categoriaId) }}
                  </span>
                  <span v-else class="flex-1 px-1 text-sm text-gray-700">
                    Selecione uma opção...
                  </span>

                  <ChevronDown :size="16" class="text-gray-600 shrink-0" />
                </div>
              </ComboboxButton>

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
                        class="bg-transparent text-xs text-white outline-none w-full placeholder:text-gray-800"
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
                      <div v-for="pai in filteredCategories" :key="pai.id">
                        <div
                          class="px-4 py-2 text-[9px] font-black text-gray-600 bg-white/1 uppercase tracking-widest flex items-center gap-2"
                        >
                          <component
                            :is="getLucideIcon(pai.icone)"
                            :size="12"
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
                            class="px-8 py-3.5 text-xs font-bold flex items-center justify-between cursor-pointer transition-colors"
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
                          class="px-4 py-3.5 text-xs font-bold flex items-center justify-between cursor-pointer transition-colors"
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
              class="text-[9px] font-black text-gray-600 uppercase tracking-widest ml-1"
              >Data</label
            >
            <div class="modal-input-group h-14.5">
              <Calendar :size="18" class="text-gray-600" />
              <input
                type="date"
                v-model="formData.data"
                required
                class="modal-input"
              />
            </div>
          </div>
          <div class="flex flex-col gap-1">
            <label
              class="text-[9px] font-black text-gray-600 uppercase tracking-widest ml-1"
              >Valor</label
            >
            <div class="modal-input-group h-14.5">
              <span class="text-xs font-black text-gray-600">R$</span>
              <input
                type="number"
                step="0.01"
                v-model="formData.valor"
                placeholder="0,00"
                required
                class="modal-input text-white font-bold"
              />
            </div>
          </div>
        </div>

        <div class="flex flex-col gap-1">
          <label
            class="text-[9px] font-black text-gray-600 uppercase tracking-widest ml-1"
            >Descrição Opcional</label
          >
          <div class="modal-input-group h-14.5">
            <input
              type="text"
              v-model="formData.descricao"
              placeholder="Ex: Mercado, Freelance..."
              class="modal-input"
            />
          </div>
        </div>

        <div class="flex gap-4 pt-6">
          <button
            type="button"
            @click="$emit('close')"
            class="flex-1 py-4 text-gray-500 font-bold hover:text-white transition-colors uppercase text-[10px] tracking-widest"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="flex-2 bg-[#6366f1] hover:bg-[#4f46e5] text-white py-4 rounded-xl font-bold transition-all shadow-lg shadow-indigo-500/20 active:scale-95 uppercase text-[10px] tracking-widest"
          >
            Confirmar Lançamento
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from "vue";
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

// Fábrica de estado inicial para facilitar o reset
const getInitialState = () => ({
  id: "",
  tipo: "D" as "R" | "D",
  data: new Date().toISOString().substring(0, 10),
  descricao: "",
  valor: "" as any,
  categoriaId: "",
});

const formData = ref(getInitialState());

// ⚡ SOLUÇÃO DO RESET E CARREGAMENTO
watch(
  () => props.show,
  (isShowing) => {
    if (isShowing) {
      if (props.mode === "edit" && props.transactionData) {
        // Popula com dados existentes
        formData.value = { ...props.transactionData };
      } else {
        // Limpa tudo para novo lançamento
        formData.value = getInitialState();
      }
      query.value = "";
    }
  },
  { immediate: true },
);

// Foco automático na busca
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

// Se mudar o tipo (R/D), reseta a categoria selecionada (apenas no modo ADD)
watch(
  () => formData.value.tipo,
  (newVal, oldVal) => {
    if (props.show && props.mode === "add" && newVal !== oldVal) {
      formData.value.categoriaId = "";
      query.value = "";
    }
  },
);

const handleSubmit = () => {
  if (!formData.value.categoriaId) {
    alert("Selecione uma categoria!");
    return;
  }
  emit("submit", {
    ...formData.value,
    valor: parseFloat(formData.value.valor),
  });
};
</script>

<style scoped>
@reference "tailwindcss";

.modal-input-group {
  @apply bg-[#18181b] p-4 rounded-2xl border border-white/5 flex items-center gap-3 transition-all focus-within:border-[#6366f1] focus-within:bg-black;
}
.modal-input {
  @apply flex-1 bg-transparent text-sm text-gray-200 outline-none placeholder:text-gray-700;
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
