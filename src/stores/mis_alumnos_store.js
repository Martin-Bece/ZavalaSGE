import { defineStore } from "pinia";
import {
  actualizar_calificacion,
  crear_calificacion,
  eliminar_calificacion,
  obtener_alumnos_comision,
  obtener_calificaciones_comision,
  obtener_comisiones_preceptor,
  obtener_materias_docente,
  actualizar_datos_alumno,
  obtener_ficha_alumno,
  descargar_constancia_alumno,
} from "@/services/mis_alumnos_service";

export const use_mis_alumnos_store = defineStore("mis_alumnos", {
  state: () => ({
    comisiones: [], // preceptor: sus comisiones
    asignaciones: [], // docente: sus materias por comisión
    comision: null, // la comisión abierta
    materia_id: null, // la materia que se está calificando (docente)
    alumnos: [], // alumnos de la comisión abierta (con sus notas si corresponde)
    cargando: false,
    guardando: false,
    alumno: null, // ficha del alumno abierto
    error: null,
  }),
  getters: {
    hay_comisiones(state) {
      return state.comisiones.length > 0;
    },
    hay_asignaciones(state) {
      return state.asignaciones.length > 0;
    },
    hay_alumnos(state) {
      return state.alumnos.length > 0;
    },
  },
  actions: {
    cargar_comisiones: async function () {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      try {
        const respuesta = await obtener_comisiones_preceptor();
        vm.comisiones = respuesta?.comisiones || [];
        return vm.comisiones;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar las comisiones.";
        vm.comisiones = [];
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    cargar_asignaciones: async function () {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      try {
        const respuesta = await obtener_materias_docente();
        vm.asignaciones = respuesta?.asignaciones || [];
        return vm.asignaciones;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar las materias.";
        vm.asignaciones = [];
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    // Lista simple de alumnos de una comisión (por ejemplo, para tomar asistencia).
    abrir_comision: async function (comision_id) {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      vm.materia_id = null;
      try {
        const respuesta = await obtener_alumnos_comision(comision_id);
        vm.comision = respuesta?.comision || null;
        vm.alumnos = respuesta?.alumnos || [];
        return vm.alumnos;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar los alumnos.";
        vm.comision = null;
        vm.alumnos = [];
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    // Alumnos con sus notas. Docente: pasar materia_id. Preceptor: se puede omitir.
    abrir_calificaciones: async function (comision_id, materia_id = null) {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      vm.materia_id = materia_id;
      try {
        const respuesta = await obtener_calificaciones_comision(
          comision_id,
          materia_id,
        );
        vm.comision = respuesta?.comision || null;
        vm.alumnos = respuesta?.alumnos || [];
        return vm.alumnos;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar las calificaciones.";
        vm.comision = null;
        vm.alumnos = [];
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    // Con id edita la nota; sin id la crea. Después recarga la misma pantalla.
    guardar_nota: async function (id, datos) {
      var vm = this;
      vm.guardando = true;
      vm.error = null;
      try {
        if (id) await actualizar_calificacion(id, datos);
        else await crear_calificacion(datos);
        await vm.abrir_calificaciones(datos.comisionId, vm.materia_id);
        return true;
      } catch (error) {
        vm.error = error.mensaje || "No se pudo guardar la nota.";
        return false;
      } finally {
        vm.guardando = false;
      }
    },

    eliminar_nota: async function (id) {
      var vm = this;
      vm.guardando = true;
      vm.error = null;
      try {
        await eliminar_calificacion(id);
        await vm.abrir_calificaciones(vm.comision?.id, vm.materia_id);
        return true;
      } catch (error) {
        vm.error = error.mensaje || "No se pudo eliminar la nota.";
        return false;
      } finally {
        vm.guardando = false;
      }
    },

    cargar_alumno: async function (id) {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      vm.alumno = null;
      try {
        const respuesta = await obtener_ficha_alumno(id);
        vm.alumno = respuesta?.alumno || null;
        return vm.alumno;
      } catch (error) {
        vm.error = error.mensaje || "No se pudo cargar el alumno.";
        return null;
      } finally {
        vm.cargando = false;
      }
    },

    guardar_datos_alumno: async function (id, datos) {
      var vm = this;
      vm.guardando = true;
      vm.error = null;
      try {
        const respuesta = await actualizar_datos_alumno(id, datos);
        vm.alumno = respuesta?.alumno || vm.alumno;
        return true;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron guardar los datos.";
        return false;
      } finally {
        vm.guardando = false;
      }
    },
  },
});
