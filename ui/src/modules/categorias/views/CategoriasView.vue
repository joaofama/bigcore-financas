<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import {
  Plus,
  Pencil,
  Trash2,
  ArrowDownCircle,
  ArrowUpCircle,
  ChevronDown,
  ChevronUp,
  ChevronLeft,
  ChevronRight,
} from "lucide-vue-next";

// Imports seguindo sua estrutura modular
import { categoriaService } from "../services/categoriaService";
import type { Categoria } from "../types";
import { getLucideIcon } from "@/shared/utils/iconMap";
import ModalCategoria from "../components/ModalCategoria.vue";

// Novos imports de notificação (Toast e Modal Elegante)
import { useToast } from "@/shared/composables/useToast";
import { useNotification } from "@/shared/composables/useNotification";

const toast = useToast();
const notify = useNotification();

// --- Estado de Dados ---
const categories = ref<Categoria[]>([]);
const expandedIds = ref<Set<string>>(new Set());

// --- Estado de Paginação ---
const itemsPerPage = 9;
const currentPageReceitas = ref(1);
const currentPageDespesas = ref(1);

// --- Estado do Modal ---
const isModalOpen = ref(false);
const modalTipo = ref<"R" | "D">("R");
const selectedParentId = ref<string | null>(null);
const selectedParentName = ref<string | null>(null);
const categoriaEmEdicao = ref<Categoria | null>(null); // Guarda os dados para edição

// --- Lógica de Carga ---
const loadCategories = async () => {
  try {
    categories.value = await categoriaService.getAll();
  } catch (error) {
    toast.error("Erro ao carregar as categorias.");
    console.error("Erro ao carregar categorias:", error);
  }
};

// --- Filtros e Paginação ---
const allReceitas = computed(() =>
  categories.value.filter((c: Categoria) => c.tipo === "R"),
);
const allDespesas = computed(() =>
  categories.value.filter((c: Categoria) => c.tipo === "D"),
);

const paginatedReceitas = computed(() => {
  const start = (currentPageReceitas.value - 1) * itemsPerPage;
  return allReceitas.value.slice(start, start + itemsPerPage);
});

const paginatedDespesas = computed(() => {
  const start = (currentPageDespesas.value - 1) * itemsPerPage;
  return allDespesas.value.slice(start, start + itemsPerPage);
});

const totalPagesReceitas = computed(() =>
  Math.ceil(allReceitas.value.length / itemsPerPage),
);
const totalPagesDespesas = computed(() =>
  Math.ceil(allDespesas.value.length / itemsPerPage),
);

// --- Ações ---
const toggleAccordion = (id: string) => {
  if (expandedIds.value.has(id)) {
    expandedIds.value.delete(id);
  } else {
    expandedIds.value.add(id);
  }
};

// Abre modal para CRIAR
const openModal = (
  tipo: "R" | "D",
  parentId: string | null = null,
  parentName: string | null = null,
) => {
  categoriaEmEdicao.value = null; // Limpa estado de edição
  modalTipo.value = tipo;
  selectedParentId.value = parentId;
  selectedParentName.value = parentName;
  isModalOpen.value = true;
};

// Abre modal para EDITAR
const openEditModal = (cat: Categoria, parentName: string | null = null) => {
  categoriaEmEdicao.value = cat; // Passa os dados para o modal
  modalTipo.value = cat.tipo;
  selectedParentId.value = cat.categoriaPaiId;
  selectedParentName.value = parentName;
  isModalOpen.value = true;
};

// Salva (Cria ou Atualiza)
const handleSaveCategory = async (data: any) => {
  try {
    if (data.id) {
      // Se tem ID, é PUT
      await categoriaService.update(data.id, {
        nome: data.nome,
        icone: data.icone,
        tipo: data.tipo,
        categoriaPaiId: data.categoriaPaiId,
      });
      toast.success("Categoria atualizada com sucesso!");
    } else {
      // Sem ID, é POST
      await categoriaService.create({
        nome: data.nome,
        icone: data.icone,
        tipo: data.tipo,
        categoriaPaiId: data.categoriaPaiId,
      });
      toast.success("Categoria incluída com sucesso!");
    }
    await loadCategories(); // Recarrega a lista do MongoDB/Redis
    isModalOpen.value = false;
  } catch (error: any) {
    const msgErro =
      error.response?.data?.message || "Ocorreu um erro ao salvar a categoria.";
    toast.error(msgErro);
    console.error("Erro ao salvar categoria:", error);
  }
};

// Lógica de Exclusão usando o notify.confirm em vez do confirm nativo
const handleDeleteCategory = (id: string) => {
  notify.confirm(
    "Esta ação não pode ser desfeita. Deseja realmente excluir esta categoria?",
    async () => {
      try {
        await categoriaService.delete(id);
        toast.success("Categoria excluída com sucesso!");
        await loadCategories(); // Força a nova listagem após limpar o cache na API
      } catch (error: any) {
        const msgErro =
          error.response?.data?.message || "Erro ao excluir a categoria.";
        toast.error(msgErro);
        console.error("Erro ao excluir:", error);
      }
    },
    "Remover Categoria?",
  );
};

onMounted(loadCategories);
</script>

<template>
  <div class="p-6 min-h-screen bg-[#09090b]">
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 h-[calc(100vh-140px)]">
      <section
        class="bg-[#121214] border border-zinc-800 rounded-xl p-6 flex flex-col h-full overflow-hidden shadow-sm"
      >
        <div class="flex justify-between items-center mb-6 shrink-0">
          <div class="flex items-center gap-3">
            <div class="p-2 bg-emerald-500/10 rounded-lg">
              <ArrowDownCircle class="text-emerald-500" :size="22" />
            </div>
            <h2
              class="text-lg font-bold uppercase tracking-widest text-zinc-100"
            >
              Receitas
            </h2>
          </div>
          <button
            @click="openModal('R')"
            class="bg-emerald-600 hover:bg-emerald-700 text-white px-4 py-1.5 rounded-lg font-bold text-xs uppercase transition-all active:scale-95"
          >
            Nova
          </button>
        </div>

        <div class="grow overflow-y-auto pr-2 custom-scrollbar space-y-3">
          <div v-for="cat in paginatedReceitas" :key="cat.id" class="group">
            <div
              class="flex items-center justify-between p-4 bg-zinc-900/40 border border-zinc-800/50 rounded-lg group-hover:border-zinc-700 transition-all"
            >
              <div class="flex items-center gap-4">
                <component
                  :is="getLucideIcon(cat.icone)"
                  class="text-emerald-500"
                  :size="20"
                />
                <span
                  class="font-bold uppercase text-sm tracking-wide text-zinc-300"
                  >{{ cat.nome }}</span
                >
              </div>
              <div
                class="flex gap-2 opacity-20 group-hover:opacity-100 transition-opacity"
              >
                <Plus
                  class="cursor-pointer hover:text-white"
                  :size="18"
                  @click.stop="openModal('R', cat.id, cat.nome)"
                />
                <Pencil
                  class="cursor-pointer hover:text-white"
                  :size="18"
                  @click.stop="openEditModal(cat)"
                />
                <Trash2
                  class="cursor-pointer hover:text-red-500"
                  :size="18"
                  @click.stop="handleDeleteCategory(cat.id)"
                />
              </div>
            </div>
          </div>
        </div>

        <div
          v-if="totalPagesReceitas > 1"
          class="flex items-center justify-center gap-4 mt-6 pt-4 border-t border-zinc-800 shrink-0"
        >
          <button
            @click="currentPageReceitas--"
            :disabled="currentPageReceitas === 1"
            class="p-1 text-zinc-500 hover:text-white disabled:opacity-20 transition-colors"
          >
            <ChevronLeft :size="20" />
          </button>
          <span
            class="text-[10px] font-bold text-zinc-500 uppercase tracking-tighter"
            >Página {{ currentPageReceitas }} de {{ totalPagesReceitas }}</span
          >
          <button
            @click="currentPageReceitas++"
            :disabled="currentPageReceitas === totalPagesReceitas"
            class="p-1 text-zinc-500 hover:text-white disabled:opacity-20 transition-colors"
          >
            <ChevronRight :size="20" />
          </button>
        </div>
      </section>

      <section
        class="bg-[#121214] border border-zinc-800 rounded-xl p-6 flex flex-col h-full overflow-hidden shadow-sm"
      >
        <div class="flex justify-between items-center mb-6 shrink-0">
          <div class="flex items-center gap-3">
            <div class="p-2 bg-red-500/10 rounded-lg">
              <ArrowUpCircle class="text-red-500" :size="22" />
            </div>
            <h2
              class="text-lg font-bold uppercase tracking-widest text-zinc-100"
            >
              Despesas
            </h2>
          </div>
          <button
            @click="openModal('D')"
            class="bg-red-600 hover:bg-red-700 text-white px-4 py-1.5 rounded-lg font-bold text-xs uppercase transition-all active:scale-95"
          >
            Nova
          </button>
        </div>

        <div class="grow overflow-y-auto pr-2 custom-scrollbar space-y-4">
          <div v-for="cat in paginatedDespesas" :key="cat.id" class="space-y-1">
            <div
              @click="toggleAccordion(cat.id)"
              class="flex items-center justify-between p-4 bg-zinc-900/40 border border-zinc-800/50 rounded-lg group cursor-pointer hover:border-zinc-700 transition-all"
            >
              <div class="flex items-center gap-4">
                <component
                  :is="getLucideIcon(cat.icone)"
                  class="text-red-500"
                  :size="20"
                />
                <span
                  class="font-bold uppercase text-sm tracking-wide text-zinc-300"
                  >{{ cat.nome }}</span
                >
                <div
                  v-if="cat.subcategorias.length > 0"
                  class="flex items-center"
                >
                  <ChevronDown
                    v-if="!expandedIds.has(cat.id)"
                    class="text-zinc-600"
                    :size="16"
                  />
                  <ChevronUp v-else class="text-zinc-600" :size="16" />
                </div>
              </div>
              <div class="flex gap-2" @click.stop>
                <Plus
                  class="cursor-pointer text-zinc-500 hover:text-white"
                  :size="18"
                  @click.stop="openModal('D', cat.id, cat.nome)"
                />
                <Pencil
                  class="cursor-pointer text-zinc-500 hover:text-white"
                  :size="18"
                  @click.stop="openEditModal(cat)"
                />
                <Trash2
                  class="cursor-pointer text-zinc-500 hover:text-red-500"
                  :size="18"
                  @click.stop="handleDeleteCategory(cat.id)"
                />
              </div>
            </div>

            <transition name="accordion">
              <div
                v-if="cat.subcategorias.length > 0 && expandedIds.has(cat.id)"
                class="overflow-hidden"
              >
                <div
                  class="ml-8 border-l-2 border-zinc-800/50 pl-4 py-2 space-y-2"
                >
                  <div
                    v-for="sub in cat.subcategorias"
                    :key="sub.id"
                    class="flex justify-between items-center group/sub py-1"
                  >
                    <div class="flex items-center gap-3">
                      <component
                        :is="getLucideIcon(sub.icone)"
                        class="text-zinc-600"
                        :size="16"
                      />
                      <span
                        class="text-zinc-500 group-hover/sub:text-zinc-200 text-xs font-bold uppercase tracking-tighter transition-colors"
                        >{{ sub.nome }}</span
                      >
                    </div>
                    <div
                      class="flex gap-2 opacity-0 group-hover/sub:opacity-100 transition-opacity"
                    >
                      <Pencil
                        class="cursor-pointer text-zinc-600 hover:text-white"
                        :size="14"
                        @click.stop="openEditModal(sub, cat.nome)"
                      />
                      <Trash2
                        class="cursor-pointer text-zinc-600 hover:text-red-500"
                        :size="14"
                        @click.stop="handleDeleteCategory(sub.id)"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </transition>
          </div>
        </div>

        <div
          v-if="totalPagesDespesas > 1"
          class="flex items-center justify-center gap-4 mt-6 pt-4 border-t border-zinc-800 shrink-0"
        >
          <button
            @click="currentPageDespesas--"
            :disabled="currentPageDespesas === 1"
            class="p-1 text-zinc-500 hover:text-white disabled:opacity-20 transition-colors"
          >
            <ChevronLeft :size="20" />
          </button>
          <span
            class="text-[10px] font-bold text-zinc-500 uppercase tracking-tighter"
            >Página {{ currentPageDespesas }} de {{ totalPagesDespesas }}</span
          >
          <button
            @click="currentPageDespesas++"
            :disabled="currentPageDespesas === totalPagesDespesas"
            class="p-1 text-zinc-500 hover:text-white disabled:opacity-20 transition-colors"
          >
            <ChevronRight :size="20" />
          </button>
        </div>
      </section>
    </div>

    <ModalCategoria
      :is-open="isModalOpen"
      :tipo="modalTipo"
      :parent-id="selectedParentId"
      :parent-name="selectedParentName"
      :categoria-para-editar="categoriaEmEdicao"
      @close="isModalOpen = false"
      @confirm="handleSaveCategory"
    />
  </div>
</template>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #27272a;
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #3f3f46;
}

.accordion-enter-active,
.accordion-leave-active {
  transition: all 0.3s ease-out;
  max-height: 800px;
}
.accordion-enter-from,
.accordion-leave-to {
  max-height: 0;
  opacity: 0;
  transform: translateY(-10px);
}
</style>
