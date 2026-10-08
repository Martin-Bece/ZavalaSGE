<template>
  <comp_page
    :titulo="elegido ? elegido.titulo : 'Calificaciones'"
    :mostrar_actualizar="true"
    @actualizar="recargar"
  >
    <div v-if="elegido" class="barra-volver">
      <ion-button fill="clear" size="small" @click="volver">
        <ion-icon slot="start" :icon="arrowBackOutline" />
        Volver
      </ion-button>
      <span class="subtitulo">{{ elegido.curso }}</span>
    </div>

    <div v-if="store.cargando" class="ion-padding">
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

    <comp_estado_error
      v-else-if="store.error"
      :mensaje="store.error"
      @reintentar="recargar"
    />

    <!-- NIVEL 2: alumnos con sus notas -->
    <comp_alumnos_comision
      v-else-if="elegido"
      :alumnos="store.alumnos"
      @abrir="abrir_alumno"
    >
      <template #acciones="{ alumno }">
        <div class="notas">
          <ion-chip
            v-for="nota in alumno.calificaciones"
            :key="nota.id"
            :color="color_nota(nota.nota)"
            class="chip-nota"
            @click="es_docente ? editar_nota(alumno, nota) : null"
          >
            <ion-label>{{ etiqueta(nota) }}</ion-label>
          </ion-chip>
          <span v-if="alumno.calificaciones.length === 0" class="sin-notas">
            Sin notas cargadas
          </span>
        </div>
        <ion-button
          v-if="es_docente"
          fill="outline"
          size="small"
          @click="nueva_nota(alumno)"
        >
          <ion-icon slot="start" :icon="addOutline" />
          Agregar nota
        </ion-button>
      </template>
    </comp_alumnos_comision>

    <!-- NIVEL 1 (docente): sus materias por comisión -->
    <template v-else-if="es_docente">
      <comp_estado_vacio
        v-if="!store.hay_asignaciones"
        titulo="Sin materias asignadas"
        mensaje="Todavía no tenés materias asignadas para calificar."
      />
      <ion-card
        v-for="asignacion in store.asignaciones"
        :key="asignacion.id"
        button
        class="card-custom"
        @click="elegir_asignacion(asignacion)"
      >
        <ion-item lines="none">
          <ion-avatar slot="start" class="avatar-icono">
            <ion-icon :icon="clipboardOutline" />
          </ion-avatar>
          <ion-label class="ion-text-wrap">
            <h2>
              <strong>{{ asignacion.materiaNombre }}</strong>
            </h2>
            <div class="chips-row">
              <ion-chip color="tertiary" class="chip-dato">
                <ion-icon :icon="layersOutline" />
                <ion-label
                  >{{ asignacion.cursoNombre }} ·
                  {{ asignacion.comisionNombre }}</ion-label
                >
              </ion-chip>
              <ion-chip color="primary" class="chip-dato">
                <ion-icon :icon="peopleOutline" />
                <ion-label>{{ asignacion.cantidadAlumnos }} alumnos</ion-label>
              </ion-chip>
            </div>
          </ion-label>
        </ion-item>
      </ion-card>
    </template>

    <!-- NIVEL 1 (preceptor): sus comisiones, con filtro por curso -->
    <template v-else>
      <div v-if="cursos.length > 1" class="segment-container">
        <ion-segment
          :value="filtro_curso || 'todos'"
          scrollable
          @ionChange="cambiar_curso($event)"
        >
          <ion-segment-button value="todos">
            <ion-label>Todos</ion-label>
          </ion-segment-button>
          <ion-segment-button
            v-for="curso in cursos"
            :key="curso"
            :value="curso"
          >
            <ion-label>{{ curso }}</ion-label>
          </ion-segment-button>
        </ion-segment>
      </div>

      <comp_estado_vacio
        v-if="!store.hay_comisiones"
        titulo="Sin comisiones asignadas"
        mensaje="Todavía no tenés comisiones a tu cargo."
      />
      <ion-card
        v-for="comision in comisiones_filtradas"
        :key="comision.id"
        button
        class="card-custom"
        @click="elegir_comision(comision)"
      >
        <ion-item lines="none">
          <ion-avatar slot="start" class="avatar-icono">
            <ion-icon :icon="clipboardOutline" />
          </ion-avatar>
          <ion-label class="ion-text-wrap">
            <h2>
              <strong>{{ comision.nombre }}</strong>
            </h2>
            <div class="chips-row">
              <ion-chip color="tertiary" class="chip-dato">
                <ion-icon :icon="ribbonOutline" />
                <ion-label>{{ comision.cursoNombre }}</ion-label>
              </ion-chip>
              <ion-chip color="primary" class="chip-dato">
                <ion-icon :icon="peopleOutline" />
                <ion-label>{{ comision.cantidadAlumnos }} alumnos</ion-label>
              </ion-chip>
            </div>
          </ion-label>
        </ion-item>
      </ion-card>
    </template>
  </comp_page>
</template>

<script>
import comp_page from "@/components/estructura/comp_page.vue";
import comp_estado_error from "@/components/base/comp_estado_error.vue";
import comp_estado_vacio from "@/components/base/comp_estado_vacio.vue";
import comp_alumnos_comision from "@/components/dominio/comp_alumnos_comision.vue";

import { use_mis_alumnos_store } from "@/stores/mis_alumnos_store";
import { use_sesion_store } from "@/stores/sesion_store";
import {
  alertController,
  IonAvatar,
  IonButton,
  IonCard,
  IonChip,
  IonIcon,
  IonItem,
  IonLabel,
  IonSegment,
  IonSegmentButton,
  IonSkeletonText,
  toastController,
} from "@ionic/vue";
import {
  addOutline,
  arrowBackOutline,
  clipboardOutline,
  layersOutline,
  peopleOutline,
  ribbonOutline,
} from "ionicons/icons";
import { avisar_local } from "@/services/notificacion_local_service";

export default {
  name: "calificaciones_page",
  components: {
    comp_page,
    comp_estado_error,
    comp_estado_vacio,
    comp_alumnos_comision,
    IonAvatar,
    IonButton,
    IonCard,
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
      // { titulo, curso, comision_id, materia_id } de la pantalla abierta
      elegido: null,
      filtro_curso: null,
      addOutline,
      arrowBackOutline,
      clipboardOutline,
      layersOutline,
      peopleOutline,
      ribbonOutline,
    };
  },
  computed: {
    store() {
      return use_mis_alumnos_store();
    },
    es_docente() {
      return use_sesion_store().rol_activo === "DOCENTE";
    },
    cursos() {
      return [...new Set(this.store.comisiones.map((c) => c.cursoNombre))];
    },
    comisiones_filtradas() {
      if (!this.filtro_curso) return this.store.comisiones;
      return this.store.comisiones.filter(
        (c) => c.cursoNombre === this.filtro_curso,
      );
    },
  },
  methods: {
    abrir_alumno(alumno) {
      this.$router.push(`/app/alumno/${alumno.id}`);
    },
    cambiar_curso(evento) {
      const valor = evento.detail.value;
      this.filtro_curso = valor === "todos" ? null : valor;
    },
    async elegir_asignacion(asignacion) {
      this.elegido = {
        titulo: `${asignacion.materiaNombre} · ${asignacion.comisionNombre}`,
        curso: asignacion.cursoNombre,
        comision_id: asignacion.comisionId,
        materia_id: asignacion.materiaId,
      };
      await this.store.abrir_calificaciones(
        this.elegido.comision_id,
        this.elegido.materia_id,
      );
    },
    async elegir_comision(comision) {
      this.elegido = {
        titulo: comision.nombre,
        curso: comision.cursoNombre,
        comision_id: comision.id,
        materia_id: null,
      };
      await this.store.abrir_calificaciones(this.elegido.comision_id, null);
    },
    volver() {
      this.elegido = null;
      this.store.comision = null;
      this.store.alumnos = [];
      this.store.error = null;
    },
    async recargar() {
      if (this.elegido) {
        await this.store.abrir_calificaciones(
          this.elegido.comision_id,
          this.elegido.materia_id,
        );
      } else if (this.es_docente) {
        await this.store.cargar_asignaciones();
      } else {
        await this.store.cargar_comisiones();
      }
    },
    etiqueta(nota) {
      // El docente ya está dentro de una materia; el preceptor ve todas, así que se aclara.
      return this.es_docente
        ? `${nota.instancia}: ${nota.nota}`
        : `${nota.materiaNombre} · ${nota.instancia}: ${nota.nota}`;
    },
    // Ajustá el 6 si la escuela aprueba con otra nota.
    color_nota(nota) {
      return Number(nota) < 6 ? "danger" : "success";
    },
    async avisar(mensaje) {
      const toast = await toastController.create({
        message: mensaje,
        duration: 2500,
        position: "bottom",
      });
      await toast.present();
    },
    nueva_nota(alumno) {
      return this.abrir_formulario_nota(alumno, null);
    },
    editar_nota(alumno, nota) {
      return this.abrir_formulario_nota(alumno, nota);
    },
    async abrir_formulario_nota(alumno, nota) {
      const editando = Boolean(nota);
      const botones = [{ text: "Cancelar", role: "cancel" }];

      if (editando) {
        botones.push({
          text: "Eliminar",
          role: "destructive",
          handler: async () => {
            const ok = await this.store.eliminar_nota(nota.id);
            if (!ok) await this.informar_error();
          },
        });
      }

      botones.push({
        text: "Guardar",
        handler: async (valores) => {
          const valor = Number(String(valores.nota ?? "").replace(",", "."));
          const instancia = String(valores.instancia ?? "").trim();
          if (!Number.isFinite(valor) || valor < 1 || valor > 10) {
            await this.avisar("La nota debe estar entre 1 y 10.");
            return false; // deja el formulario abierto
          }
          if (!instancia) {
            await this.avisar("Indicá la instancia (por ejemplo, Parcial 1).");
            return false;
          }
          const ok = await this.store.guardar_nota(editando ? nota.id : null, {
            alumnoId: alumno.id,
            materiaId: this.elegido.materia_id,
            comisionId: this.elegido.comision_id,
            nota: valor,
            instancia,
          });
          if (!ok) {
            await this.informar_error();
            return;
          }
          // Aviso del propio teléfono: responde a algo que el usuario acaba de hacer.
          const aviso = await avisar_local(
            editando ? "Nota modificada" : "Nota guardada",
            `${alumno.apellido}, ${alumno.nombre} · ${instancia}: ${valor}`,
          );
          if (aviso.mensaje) await this.avisar(aviso.mensaje);
        },
      });

      const alerta = await alertController.create({
        header: editando ? "Editar nota" : "Nueva nota",
        subHeader: `${alumno.apellido}, ${alumno.nombre}`,
        inputs: [
          {
            name: "nota",
            type: "number",
            placeholder: "Nota (1 a 10)",
            value: editando ? String(nota.nota) : "",
            attributes: { min: 1, max: 10, step: 0.5, inputmode: "decimal" },
          },
          {
            name: "instancia",
            type: "text",
            placeholder: "Instancia (Parcial 1, TP...)",
            value: editando ? nota.instancia : "",
          },
        ],
        buttons: botones,
      });
      await alerta.present();
    },
    // Si falla un guardado o borrado, el error se muestra aparte para no tapar la lista.
    async informar_error() {
      await this.avisar(this.store.error || "No se pudo completar la acción.");
      this.store.error = null;
    },
  },
  // Recarga lo que se está mirando en vez de volver siempre a la lista.
  ionViewDidEnter() {
    this.recargar();
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

.avatar-icono {
  background-color: var(--ion-color-success);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.3rem;
}

.chips-row,
.notas {
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

.chip-nota {
  margin: 0;
}

.sin-notas {
  opacity: 0.6;
  font-size: 0.9rem;
}

.barra-volver {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px 0;
}

.subtitulo {
  opacity: 0.8;
  font-size: 0.95rem;
}

.segment-container {
  padding: 0 16px;
  margin-top: 8px;
}

ion-segment {
  --background: rgba(255, 255, 255, 0.05);
  border-radius: 10px;
  padding: 4px;
}

ion-segment-button {
  --color: #94a3b8;
  --color-checked: var(--ion-color-primary);
  --indicator-color: rgba(255, 255, 255, 0.1);
  font-weight: 600;
}
</style>
