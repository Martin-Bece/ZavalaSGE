<template>
  <ion-modal
    :is-open="abierto"
    @didDismiss="$emit('cerrar')"
    class="modal-persona"
  >
    <ion-header class="ion-no-border">
      <ion-toolbar color="primary">
        <ion-title>{{
          persona ? "Editar " + obtenerTituloRol : "Nuevo " + obtenerTituloRol
        }}</ion-title>
        <ion-buttons slot="end">
          <ion-button type="button" @click="$emit('cerrar')">CERRAR</ion-button>
        </ion-buttons>
      </ion-toolbar>
    </ion-header>

    <ion-content class="ion-padding modal-content">
      <!-- Agregamos form con @submit.prevent para evitar que la interfaz salte/se desplace -->
      <form @submit.prevent="guardar">
        <ion-list class="form-list">
          <!-- 1. SELECCIÓN DE ROL -->
          <ion-item class="form-item">
            <ion-select
              v-model="formulario.rol"
              label="Tipo de Persona *"
              label-placement="stacked"
              placeholder="Seleccione un rol"
              :disabled="Boolean(persona)"
            >
              <ion-select-option value="docente">Docente</ion-select-option>
              <ion-select-option value="preceptor">Preceptor</ion-select-option>
              <ion-select-option value="alumno">Alumno</ion-select-option>
            </ion-select>
          </ion-item>

          <!-- 2. DATOS GENERALES DE LA PERSONA -->
          <ion-item-divider color="light" class="divider-section">
            <ion-label>Datos Personales</ion-label>
          </ion-item-divider>

          <ion-item class="form-item">
            <ion-input
              v-model="formulario.nombre"
              label="Nombre *"
              label-placement="stacked"
              placeholder="Ej: Juan Carlos"
            />
          </ion-item>

          <ion-item class="form-item">
            <ion-input
              v-model="formulario.apellido"
              label="Apellido *"
              label-placement="stacked"
              placeholder="Ej: Gómez"
            />
          </ion-item>

          <ion-item class="form-item">
            <ion-input
              v-model="formulario.dni"
              label="DNI *"
              label-placement="stacked"
              placeholder="Sin puntos"
            />
          </ion-item>

          <ion-item class="form-item">
            <ion-input
              v-model="formulario.telefono"
              label="Teléfono"
              label-placement="stacked"
              placeholder="Ej: 2664123456"
            />
          </ion-item>

          <ion-item class="form-item">
            <ion-input
              v-model="formulario.direccion"
              label="Dirección"
              label-placement="stacked"
              placeholder="Ej: Av. España 1234"
            />
          </ion-item>

          <!-- FECHA DE NACIMIENTO -->
          <ion-item class="form-item">
            <ion-input
              type="date"
              :value="formulario.fecha_nacimiento"
              @ionInput="formulario.fecha_nacimiento = $event.detail.value"
              label="Fecha de Nacimiento"
              label-placement="stacked"
            />
          </ion-item>

          <!-- 3. CAMPOS ESPECÍFICOS SEGÚN ROL -->
          <template v-if="formulario.rol === 'docente'">
            <ion-item-divider color="light" class="divider-section">
              <ion-label>Datos de Docente</ion-label>
            </ion-item-divider>
            <ion-item class="form-item">
              <ion-input
                v-model="formulario.legajo"
                label="Legajo *"
                label-placement="stacked"
                placeholder="Ej: DOC-1001"
              />
            </ion-item>
            <ion-item class="form-item" lines="none">
              <ion-input
                v-model="formulario.titulo"
                label="Título"
                label-placement="stacked"
                placeholder="Ej: Ingeniero en Sistemas"
              />
            </ion-item>
          </template>

          <template v-if="formulario.rol === 'preceptor'">
            <ion-item-divider color="light" class="divider-section">
              <ion-label>Datos de Preceptor</ion-label>
            </ion-item-divider>
            <ion-item class="form-item" lines="none">
              <ion-input
                v-model="formulario.legajo"
                label="Legajo *"
                label-placement="stacked"
                placeholder="Ej: PREC-2001"
              />
            </ion-item>
          </template>

          <template v-if="formulario.rol === 'alumno'">
            <ion-item-divider color="light" class="divider-section">
              <ion-label>Datos de Alumno</ion-label>
            </ion-item-divider>
            <ion-item class="form-item" lines="none">
              <ion-input
                v-model="formulario.legajo"
                label="Legajo Alumno *"
                label-placement="stacked"
                placeholder="Ej: ALU-3001"
              />
            </ion-item>
          </template>
        </ion-list>

        <ion-note v-if="error" color="danger" class="error-note">{{
          error
        }}</ion-note>

        <div class="button-container">
          <!-- Cambiado type="submit" para que responsa al submit del formulario -->
          <ion-button
            type="submit"
            expand="block"
            color="primary"
            class="btn-guardar"
            :disabled="guardando || !formulario.rol"
          >
            <ion-spinner v-if="guardando" name="crescent" />
            <span v-else>GUARDAR</span>
          </ion-button>
        </div>
      </form>
    </ion-content>
  </ion-modal>
</template>

<script>
import {
  IonButton,
  IonButtons,
  IonContent,
  IonHeader,
  IonInput,
  IonItem,
  IonItemDivider,
  IonLabel,
  IonList,
  IonModal,
  IonNote,
  IonSelect,
  IonSelectOption,
  IonSpinner,
  IonTitle,
  IonToolbar,
} from "@ionic/vue";

const FORMULARIO_VACIO = {
  rol: null,
  nombre: "",
  apellido: "",
  dni: "",
  telefono: "",
  direccion: "",
  fecha_nacimiento: "",
  legajo: "",
  titulo: "",
};

export default {
  name: "comp_persona_formulario_modal",
  components: {
    IonButton,
    IonButtons,
    IonContent,
    IonHeader,
    IonInput,
    IonItem,
    IonItemDivider,
    IonLabel,
    IonList,
    IonModal,
    IonNote,
    IonSelect,
    IonSelectOption,
    IonSpinner,
    IonTitle,
    IonToolbar,
  },
  emits: ["cerrar", "guardar"],
  props: {
    abierto: {
      type: Boolean,
      default: false,
    },
    persona: {
      type: Object,
      default: null,
    },
    rolPredeterminado: {
      type: String,
      default: null,
    },
    error: {
      type: String,
      default: "",
    },
    guardando: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      formulario: { ...FORMULARIO_VACIO },
    };
  },
  computed: {
    obtenerTituloRol() {
      if (!this.formulario.rol) return "Persona";
      return (
        this.formulario.rol.charAt(0).toUpperCase() +
        this.formulario.rol.slice(1)
      );
    },
  },
  methods: {
    formatearFecha(fechaRaw) {
      if (!fechaRaw) return "";
      if (typeof fechaRaw === "string") {
        return fechaRaw.split("T")[0];
      }
      return fechaRaw;
    },
    guardar(e) {
      if (e) e.preventDefault();

      console.log("1. Botón guardar presionado en el modal.");
      console.log("2. Datos del formulario:", this.formulario);

      // Verificamos si estamos editando y extraemos ambos IDs con seguridad
      const esEdicion = Boolean(this.persona);

      // ID del rol específico (Docente, Alumno o Preceptor) que viene en la prop persona
      const idRol = esEdicion ? this.persona.id || this.persona.idRol : null;

      // ID de la tabla Persona base (vital para no romper la relación ni duplicar DNI)
      const idPersonaReal = esEdicion
        ? this.persona.personaId ||
          this.persona.idPersona ||
          this.persona.persona_id
        : null;

      const datosForm = { ...this.formulario };

      if (!esEdicion) {
        delete datosForm.id;
        delete datosForm.personaId;
      } else {
        datosForm.id = idRol;
        datosForm.personaId = idPersonaReal;
      }

      this.$emit("guardar", {
        id: idRol, // Este es el ID que usa actualizar_docente(id, ...)
        rol: this.formulario.rol,
        datos: datosForm, // Aquí viaja adentro el personaId real
      });
    },
  },
  watch: {
    abierto(nuevoEstado) {
      if (!nuevoEstado) return;

      if (this.persona) {
        const rolDetectado = (
          this.persona.rol ||
          this.persona.tipo ||
          "docente"
        ).toLowerCase();

        let nombre = this.persona.nombre || this.persona.personaNombre || "";
        let apellido =
          this.persona.apellido || this.persona.personaApellido || "";

        if (!nombre && !apellido && this.persona.nombreCompleto) {
          const partes = this.persona.nombreCompleto.trim().split(" ");
          apellido = partes.length > 1 ? partes.pop() : "";
          nombre = partes.join(" ");
        }

        const fechaRaw =
          this.persona.fecha_nacimiento || this.persona.fechaNacimiento || "";

        this.formulario = {
          rol: rolDetectado,
          nombre: nombre,
          apellido: apellido,
          dni:
            this.persona.dni ||
            this.persona.personaDni ||
            this.persona.documento ||
            "",
          telefono: this.persona.telefono || this.persona.personaTelefono || "",
          direccion:
            this.persona.direccion || this.persona.direccionPersona || "",
          fecha_nacimiento: this.formatearFecha(fechaRaw),
          legajo: this.persona.legajo || "",
          titulo: this.persona.titulo || "",
          // Guardamos los IDs dentro del formulario por si el store los necesita limpiar o rutear
          id: this.persona.id || null,
          personaId:
            this.persona.personaId ||
            this.persona.idPersona ||
            this.persona.persona_id ||
            null,
        };
      } else {
        const rolInicial =
          this.rolPredeterminado && this.rolPredeterminado !== "todos"
            ? this.rolPredeterminado.toLowerCase()
            : "docente";

        this.formulario = {
          ...FORMULARIO_VACIO,
          rol: rolInicial,
        };
      }
    },
  },
};
</script>

<style scoped>
.modal-content {
  --background: var(--ion-background-color, #f4f6f9);
}

.form-list {
  background: transparent;
  padding: 0;
}

.form-item {
  --background: var(--ion-card-background, #ffffff);
  --border-radius: 10px;
  --padding-start: 14px;
  margin-bottom: 10px;
  border: 1px solid rgba(0, 0, 0, 0.08);
}

body.dark .form-item {
  border-color: rgba(255, 255, 255, 0.1);
}

.divider-section {
  --background: transparent;
  font-weight: 700;
  color: var(--ion-color-primary);
  margin-top: 15px;
  margin-bottom: 5px;
  text-transform: uppercase;
  font-size: 0.78rem;
  letter-spacing: 0.5px;
}

.button-container {
  margin-top: 24px;
  margin-bottom: 16px;
}

.btn-guardar {
  --border-radius: 10px;
  font-weight: 700;
  height: 48px;
  box-shadow: 0 4px 12px rgba(0, 71, 171, 0.25);
}

.error-note {
  display: block;
  margin-top: 10px;
  font-weight: 600;
}
</style>
