<template>
  <comp_page titulo="Gestión de Personal">
    <!-- 1. Buscador Genérico -->
    <comp_buscador
      v-model="store.filtros.busqueda"
      placeholder="Buscar por DNI o nombre..."
      @buscar="alBuscar"
    />

    <!-- Filtro por Rol (Pestañas / Segments) -->
    <div class="segment-container">
      <ion-segment
        :value="store.filtros.rol || 'todos'"
        scrollable
        @ionChange="alCambiarRol($event)"
      >
        <ion-segment-button value="todos">
          <ion-label>Todos</ion-label>
        </ion-segment-button>
        <ion-segment-button value="docente">
          <ion-label>Docentes</ion-label>
        </ion-segment-button>
        <ion-segment-button value="preceptor">
          <ion-label>Preceptores</ion-label>
        </ion-segment-button>
      </ion-segment>
    </div>

    <!-- Botón para Alta -->
    <div class="ion-padding-horizontal ion-margin-top">
      <ion-button expand="block" @click="abrirModalCrear">
        <ion-icon slot="start" :icon="addOutline" />
        Nueva Persona
      </ion-button>
    </div>

    <!-- 2. Estado Carga (Esqueleto) -->
    <div v-if="store.cargando && !store.hay_personas" class="ion-padding">
      <ion-card v-for="i in 3" :key="i" class="card-custom">
        <ion-item lines="none">
          <ion-avatar slot="start">
            <ion-skeleton-text animated />
          </ion-avatar>
          <ion-label>
            <ion-skeleton-text animated style="width: 60%; height: 20px" />
            <ion-skeleton-text
              animated
              style="width: 40%; height: 14px; margin-top: 6px"
            />
          </ion-label>
        </ion-item>
      </ion-card>
    </div>

    <!-- 3. Estado Error -->
    <comp_estado_error
      v-else-if="store.error"
      :mensaje="store.error"
      @reintentar="store.cargar()"
    />

    <!-- 4. Estado Vacío -->
    <comp_estado_vacio
      v-else-if="!store.cargando && !store.hay_personas"
      titulo="Sin registros"
      mensaje="No se encontraron personas cargadas o que coincidan con la búsqueda."
    />

    <!-- 5. Lista con Datos -->
    <comp_lista
      :cargando="store.cargando"
      :hay_mas="
        store.pagina ? store.pagina.hayMas : store.hayMas || store.hay_mas
      "
      :mostrados="store.personas.length"
      :total="store.pagina ? store.pagina.total : store.total"
      @cargar_mas="store.cargar_mas()"
    >
      <ion-card
        v-for="persona in store.personas"
        :key="`${persona.rol}-${persona.id}`"
        class="card-custom"
      >
        <ion-item lines="none">
          <ion-avatar slot="start" class="avatar-iniciales">
            <span>{{
              obtenerIniciales(
                persona.nombreCompleto ||
                  persona.nombre ||
                  persona.personaNombre,
              )
            }}</span>
          </ion-avatar>
          <ion-label class="ion-text-wrap">
            <h2>
              <strong>
                {{
                  persona.nombreCompleto ||
                  (persona.nombre
                    ? persona.nombre + ", " + persona.apellido
                    : null) ||
                  (persona.personaNombre
                    ? persona.personaNombre + ", " + persona.apellido
                    : null) ||
                  "Sin Nombre"
                }}
              </strong>
            </h2>

            <div class="chips-row">
              <ion-chip color="primary" class="chip-dni">
                <ion-icon :icon="cardOutline" />
                <ion-label
                  >DNI:
                  {{
                    persona.dni || persona.personaDni || persona.documento
                  }}</ion-label
                >
              </ion-chip>

              <!-- Badge Dinámico según Rol -->
              <ion-badge
                v-if="persona.rol"
                :color="obtenerColorRol(persona.rol)"
                mode="ios"
                class="badge-rol"
              >
                {{ persona.rol }}
              </ion-badge>
            </div>
          </ion-label>
        </ion-item>

        <ion-card-content class="card-contenido">
          <div class="info-row" v-if="persona.legajo">
            <ion-icon :icon="cardOutline" class="info-icon" />
            <span>Legajo: {{ persona.legajo }}</span>
          </div>

          <div class="info-row" v-if="persona.titulo">
            <ion-icon :icon="locationOutline" class="info-icon" />
            <span>Título: {{ persona.titulo }}</span>
          </div>

          <!-- Botones de Acción (Modificación / Baja) -->
          <div class="acciones-row ion-margin-top">
            <ion-button
              fill="clear"
              size="small"
              color="primary"
              @click="abrirModalEditar(persona)"
            >
              <ion-icon slot="start" :icon="createOutline" />
              Editar
            </ion-button>
            <ion-button
              fill="clear"
              size="small"
              color="danger"
              @click="confirmarEliminar(persona)"
            >
              <ion-icon slot="start" :icon="trashOutline" />
              Eliminar
            </ion-button>
          </div>
        </ion-card-content>
      </ion-card>
    </comp_lista>

    <!-- Modal Formulario (Alta / Edición) -->
    <comp_persona_formulario_modal
      :abierto="modalAbierto"
      :persona="personaSeleccionada"
      :rol-predeterminado="store.filtros.rol"
      @cerrar="modalAbierto = false"
      @guardar="guardarPersona"
    />
  </comp_page>
</template>

<script>
import comp_page from "@/components/estructura/comp_page.vue";
import comp_buscador from "@/components/base/comp_buscador.vue";
import comp_estado_error from "@/components/base/comp_estado_error.vue";
import comp_estado_vacio from "@/components/base/comp_estado_vacio.vue";
import comp_lista from "@/components/base/comp_lista.vue";
import comp_persona_formulario_modal from "@/components/dominio/comp_persona_formulario_modal.vue";

import { use_personas_store } from "@/stores/personas_store";
import {
  alertController,
  IonAvatar,
  IonBadge,
  IonButton,
  IonCard,
  IonCardContent,
  IonChip,
  IonIcon,
  IonItem,
  IonLabel,
  IonSegment,
  IonSegmentButton,
  IonSkeletonText,
} from "@ionic/vue";
import {
  addOutline,
  cardOutline,
  callOutline,
  createOutline,
  locationOutline,
  trashOutline,
} from "ionicons/icons";

export default {
  name: "personal_page",
  components: {
    comp_page,
    comp_buscador,
    comp_estado_error,
    comp_estado_vacio,
    comp_lista,
    comp_persona_formulario_modal,
    IonAvatar,
    IonBadge,
    IonButton,
    IonCard,
    IonCardContent,
    IonChip,
    IonIcon,
    IonItem,
    IonLabel,
    IonSegment,
    IonSegmentButton,
    IonSkeletonText,
  },
  data() {
    return {
      addOutline,
      cardOutline,
      callOutline,
      createOutline,
      locationOutline,
      trashOutline,
      // Control de estado del Modal
      modalAbierto: false,
      personaSeleccionada: null,
    };
  },
  computed: {
    store() {
      return use_personas_store();
    },
  },
  methods: {
    alBuscar() {
      this.store.cargar();
    },
    alCambiarRol(event) {
      const val = event.detail.value;
      this.store.filtros.rol = val === "todos" ? null : val;
      this.store.cargar();
    },
    obtenerIniciales(texto) {
      if (!texto) return "??";
      const partes = texto.trim().split(/[\s,]+/);
      if (partes.length >= 2) {
        return `${partes[0].charAt(0)}${partes[1].charAt(0)}`.toUpperCase();
      }
      return texto.substring(0, 2).toUpperCase();
    },
    obtenerColorRol(rol) {
      switch (rol?.toLowerCase()) {
        case "docente":
          return "tertiary";
        case "preceptor":
          return "success";
        default:
          return "medium";
      }
    },
    abrirModalCrear() {
      this.personaSeleccionada = null;
      this.modalAbierto = true;
    },
    abrirModalEditar(persona) {
      this.personaSeleccionada = persona;
      this.modalAbierto = true;
    },
    async guardarPersona(payload) {
      try {
        const exito = await this.store.guardar(
          payload.id,
          payload.datos,
          payload.rol,
        );
        if (exito) {
          this.modalAbierto = false;
        }
      } catch (error) {
        console.error("Error al guardar la persona:", error);
      }
    },
    async confirmarEliminar(persona) {
      const nombreMostrar =
        persona.nombreCompleto ||
        persona.nombre ||
        persona.personaNombre ||
        "esta persona";
      const alert = await alertController.create({
        header: "Confirmar eliminación",
        message: `¿Deseas eliminar a ${nombreMostrar}?`,
        buttons: [
          {
            text: "Cancelar",
            role: "cancel",
          },
          {
            text: "Eliminar",
            role: "destructive",
            handler: async () => {
              await this.store.eliminar(persona);
            },
          },
        ],
      });

      await alert.present();
    },
  },
  ionViewDidEnter() {
    this.store.cargar();
  },
};
</script>

<style scoped>
.segment-container {
  padding: 0 16px;
  margin-top: 8px;
}

ion-segment {
  --background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  padding: 4px;
}

/* Forzar color de texto e ítems inactivos del segmento */
ion-segment-button {
  --color: #94a3b8;
  --color-checked: var(--ion-color-primary);
  --indicator-color: rgba(255, 255, 255, 0.1);
  font-weight: 600;
}

body.dark ion-segment-button {
  --color: #94a3b8;
  --color-checked: #ffffff;
}

/* Tarjetas y separación */
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

.chip-dni {
  margin: 0;
  height: 24px;
  font-size: 0.8rem;
}

.badge-rol {
  text-transform: capitalize;
  padding: 4px 8px;
  font-size: 0.75rem;
  border-radius: 6px;
}

.card-contenido {
  padding-top: 0;
}

.info-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 8px;
  color: var(--ion-text-color);
  opacity: 0.85;
  font-size: 0.95rem;
}

.info-icon {
  font-size: 1.1rem;
  color: var(--ion-color-primary);
}

.acciones-row {
  display: flex;
  justify-content: flex-end;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
  padding-top: 8px;
}
</style>
