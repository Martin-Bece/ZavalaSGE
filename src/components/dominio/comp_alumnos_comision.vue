<template>
  <div>
    <comp_buscador
      v-model="busqueda"
      placeholder="Buscar por apellido, nombre o DNI..."
    />

    <comp_estado_vacio
      v-if="alumnos.length === 0"
      titulo="Sin alumnos"
      mensaje="Esta comisión todavía no tiene alumnos inscriptos."
    />
    <comp_estado_vacio
      v-else-if="filtrados.length === 0"
      titulo="Sin resultados"
      mensaje="Ningún alumno coincide con la búsqueda."
    />

    <ion-card v-for="alumno in filtrados" :key="alumno.id" class="card-custom">
      <ion-item
        lines="none"
        button
        :detail="true"
        @click="$emit('abrir', alumno)"
      >
        <ion-avatar slot="start" class="avatar-iniciales">
          <span>{{ iniciales(alumno) }}</span>
        </ion-avatar>
        <ion-label class="ion-text-wrap">
          <h2>
            <strong>{{ alumno.apellido }}, {{ alumno.nombre }}</strong>
          </h2>
          <div class="chips-row">
            <ion-chip color="primary" class="chip-dato">
              <ion-icon :icon="cardOutline" />
              <ion-label>DNI: {{ alumno.dni }}</ion-label>
            </ion-chip>
            <ion-chip color="medium" class="chip-dato">
              <ion-label>Legajo: {{ alumno.legajo }}</ion-label>
            </ion-chip>
            <ion-badge
              v-if="alumno.estado !== 'regular'"
              color="warning"
              mode="ios"
              class="badge-estado"
            >
              {{ alumno.estado }}
            </ion-badge>
          </div>
        </ion-label>
      </ion-item>

      <ion-card-content v-if="$slots.acciones" class="card-contenido">
        <slot name="acciones" :alumno="alumno" />
      </ion-card-content>
    </ion-card>
  </div>
</template>

<script>
import comp_buscador from "@/components/base/comp_buscador.vue";
import comp_estado_vacio from "@/components/base/comp_estado_vacio.vue";
import {
  IonAvatar,
  IonBadge,
  IonCard,
  IonCardContent,
  IonChip,
  IonIcon,
  IonItem,
  IonLabel,
} from "@ionic/vue";
import { cardOutline } from "ionicons/icons";

// Sin acentos y en minúscula, para que "perez" encuentre "Pérez".
function normalizar(texto) {
  return String(texto || "")
    .toLowerCase()
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "");
}

// Lista de alumnos de una comisión. No sabe de dónde vienen ni qué se hace con ellos: cada
// pantalla pone sus acciones (asistencia, notas) en el slot "acciones".
export default {
  name: "comp_alumnos_comision",
  components: {
    comp_buscador,
    comp_estado_vacio,
    IonAvatar,
    IonBadge,
    IonCard,
    IonCardContent,
    IonChip,
    IonIcon,
    IonItem,
    IonLabel,
  },
  emits: ["abrir"],
  props: {
    alumnos: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {
      busqueda: "",
      cardOutline,
    };
  },
  computed: {
    filtrados() {
      const texto = normalizar(this.busqueda).trim();
      if (!texto) return this.alumnos;
      return this.alumnos.filter((a) =>
        normalizar(`${a.apellido} ${a.nombre} ${a.dni} ${a.legajo}`).includes(
          texto,
        ),
      );
    },
  },
  methods: {
    iniciales(alumno) {
      const a = (alumno.apellido || "").charAt(0);
      const n = (alumno.nombre || "").charAt(0);
      return `${a}${n}`.toUpperCase() || "??";
    },
  },
};
</script>

<style scoped>
.card-custom {
  margin: 12px 16px;
  border-radius: 14px;
  background: var(--ion-card-background, #ffffff);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.05);
}

body.dark .card-custom {
  background: #1e293b;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.avatar-iniciales {
  background-color: var(--ion-color-primary);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 1.1rem;
}

.chips-row {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
  margin-top: 6px;
}

.chip-dato {
  margin: 0;
  height: 24px;
  font-size: 0.8rem;
}

.badge-estado {
  text-transform: capitalize;
  padding: 4px 8px;
  font-size: 0.75rem;
  border-radius: 6px;
}

.card-contenido {
  padding-top: 0;
}
</style>
