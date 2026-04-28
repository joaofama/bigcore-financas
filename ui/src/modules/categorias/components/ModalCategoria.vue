<script setup lang="ts">
import { ref, watch, computed, reactive } from "vue";
import { ExternalLink, Info } from "lucide-vue-next";
import { getLucideIcon } from "@/shared/utils/iconMap";
import type { Categoria } from "../types";

const props = defineProps<{
  isOpen: boolean;
  tipo: "R" | "D";
  parentId?: string | null;
  parentName?: string | null;
  categoriaParaEditar?: Categoria | null;
}>();

const emit = defineEmits(["close", "confirm"]);

const nome = ref("");
const iconeDigitado = ref("Tag");

// Estado de erros para validação
const errors = reactive({
  nome: "",
  icone: "",
});

const isSubcategory = computed(
  () => !!props.parentId || !!props.categoriaParaEditar?.categoriaPaiId,
);
const isEditing = computed(() => !!props.categoriaParaEditar);

// Preenche os campos quando o modal abre para edição e limpa erros
watch(
  () => props.isOpen,
  (isOpen) => {
    if (isOpen) {
      errors.nome = "";
      errors.icone = "";

      if (props.categoriaParaEditar) {
        nome.value = props.categoriaParaEditar.nome;
        iconeDigitado.value = props.categoriaParaEditar.icone;
      } else {
        nome.value = "";
        iconeDigitado.value = "Tag";
      }
    }
  },
);

const validate = () => {
  let isValid = true;
  errors.nome = "";
  errors.icone = "";

  if (!nome.value || nome.value.trim() === "") {
    errors.nome = "O nome da categoria é obrigatório.";
    isValid = false;
  }

  if (!iconeDigitado.value || iconeDigitado.value.trim() === "") {
    errors.icone = "O ícone é obrigatório.";
    isValid = false;
  }

  return isValid;
};

const handleConfirm = () => {
  if (!validate()) return;

  emit("confirm", {
    id: props.categoriaParaEditar?.id,
    nome: nome.value.trim(),
    icone: iconeDigitado.value.trim(),
    tipo: props.tipo,
    categoriaPaiId: isEditing.value
      ? props.categoriaParaEditar!.categoriaPaiId
      : props.parentId || null,
  });

  close();
};

const close = () => {
  nome.value = "";
  iconeDigitado.value = "Tag";
  errors.nome = "";
  errors.icone = "";
  emit("close");
};
</script>

<template>
  <Transition name="fade">
    <div
      v-if="isOpen"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-sm"
    >
      <div
        class="bg-[#18181b] w-full max-w-md rounded-4xl border border-zinc-800 p-8 shadow-2xl overflow-hidden"
      >
        <div class="mb-8">
          <h2 class="text-2xl font-bold text-white">
            {{ isEditing ? "Editar" : "Nova" }}
            {{ isSubcategory ? "Subcategoria" : "Categoria" }}
          </h2>
          <p
            v-if="isSubcategory && parentName"
            class="text-xs text-purple-400 font-bold uppercase tracking-tighter mt-1"
          >
            Vinculada a: {{ parentName }}
          </p>
        </div>

        <div class="space-y-1 mb-6">
          <label
            class="text-[10px] font-bold uppercase tracking-widest text-zinc-500 ml-1"
          >
            Nome da Categoria
          </label>
          <input
            v-model="nome"
            @input="errors.nome = ''"
            type="text"
            placeholder="Ex: Alimentação, Lazer..."
            :class="[
              'w-full bg-[#1c1c1f] border-2 rounded-2xl px-5 py-4 text-white placeholder:text-zinc-600 focus:outline-none transition-all',
              errors.nome
                ? 'border-red-500/50 focus:border-red-500'
                : 'border-zinc-800 focus:border-purple-600',
            ]"
          />
          <p
            v-if="errors.nome"
            class="text-[10px] text-red-400 font-bold ml-1 uppercase tracking-wider"
          >
            {{ errors.nome }}
          </p>
        </div>

        <div class="space-y-4">
          <div class="flex justify-between items-end px-1">
            <label
              class="text-[10px] font-bold uppercase tracking-widest text-zinc-500"
            >
              Nome do Ícone (Lucide)
            </label>
            <a
              href="https://lucide.dev/icons"
              target="_blank"
              class="text-[10px] text-purple-400 hover:text-purple-300 flex items-center gap-1 transition-colors underline underline-offset-4 font-bold"
            >
              Procurar ícones <ExternalLink :size="10" />
            </a>
          </div>

          <div class="space-y-1">
            <input
              v-model="iconeDigitado"
              @input="errors.icone = ''"
              type="text"
              placeholder="Ex: Pizza, Wallet, Heart..."
              :class="[
                'w-full bg-[#1c1c1f] border-2 rounded-2xl px-5 py-4 text-white outline-none transition-all font-mono text-sm',
                errors.icone
                  ? 'border-red-500/50 focus:border-red-500'
                  : 'border-zinc-800 focus:border-zinc-700',
              ]"
            />
            <p
              v-if="errors.icone"
              class="text-[10px] text-red-400 font-bold ml-1 uppercase tracking-wider"
            >
              {{ errors.icone }}
            </p>
          </div>

          <div
            class="bg-[#1c1c1f] border border-zinc-800 rounded-2xl p-6 flex flex-col items-center justify-center gap-3"
          >
            <div
              class="p-4 bg-zinc-900 rounded-2xl border border-zinc-800 shadow-inner"
            >
              <component
                :is="getLucideIcon(iconeDigitado)"
                class="text-purple-500"
                :size="48"
              />
            </div>
            <p
              class="text-[10px] text-zinc-500 uppercase font-bold tracking-widest"
            >
              Prévia no sistema
            </p>
          </div>

          <div class="flex items-start gap-2 px-2">
            <Info class="text-zinc-600 shrink-0 mt-0.5" :size="14" />
            <p class="text-[10px] text-zinc-600 leading-tight">
              Digite o nome do ícone exatamente como aparece no site do Lucide.
              A prévia acima mostrará o ícone padrão se o nome for inválido.
            </p>
          </div>
        </div>

        <div class="flex items-center justify-end gap-6 mt-10">
          <button
            type="button"
            @click="close"
            class="text-zinc-500 hover:text-zinc-300 font-bold text-sm transition-colors uppercase tracking-widest text-[12px]"
          >
            Cancelar
          </button>
          <button
            type="button"
            @click="handleConfirm"
            class="bg-purple-600 hover:bg-purple-500 text-white px-10 py-4 rounded-xl font-bold transition-all shadow-lg shadow-purple-900/20 active:scale-95 uppercase tracking-widest text-[12px]"
          >
            Guardar
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
/* Animação Fade e Scale para o Modal */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
</style>
