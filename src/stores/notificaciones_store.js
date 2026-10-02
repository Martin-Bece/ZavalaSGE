import { defineStore } from "pinia";
import { sin_vibrar } from "@/services/ajax_service";
import {
  marcar_leida as marcar_leida_api,
  marcar_todas_leidas as marcar_todas_leidas_api,
  obtener_no_leidas,
  obtener_notificaciones,
} from "@/services/notificaciones_service";

// Cada cuántos milisegundos se consulta el contador de la campana.
const INTERVALO_MS = 30000;
let temporizador = null;

export const use_notificaciones_store = defineStore("notificaciones", {
  state: () => ({
    notificaciones: [],
    no_leidas: 0,
    cargando: false,
    error: null,
  }),
  getters: {
    hay_notificaciones(state) {
      return state.notificaciones.length > 0;
    },
  },
  actions: {
    cargar: async function () {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      try {
        const respuesta = await obtener_notificaciones();
        vm.notificaciones = respuesta?.notificaciones || [];
        vm.no_leidas = respuesta?.noLeidas ?? 0;
        return vm.notificaciones;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar las notificaciones.";
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    // Solo el número del globo. Si falla (sin conexión), el globo queda como estaba.
    refrescar_contador: async function () {
      var vm = this;
      try {
        const respuesta = await sin_vibrar(() => obtener_no_leidas());
        vm.no_leidas = respuesta?.noLeidas ?? 0;
      } catch {
        // silencio: no hay nada que mostrar por un contador que no se pudo actualizar
      }
    },

    marcar_leida: async function (id) {
      var vm = this;
      try {
        await marcar_leida_api(id);
        const item = vm.notificaciones.find((n) => n.id === id);
        if (item && !item.leida) {
          item.leida = true;
          vm.no_leidas = Math.max(0, vm.no_leidas - 1);
        }
        return true;
      } catch {
        return false;
      }
    },

    marcar_todas: async function () {
      var vm = this;
      try {
        await marcar_todas_leidas_api();
        vm.notificaciones.forEach((n) => (n.leida = true));
        vm.no_leidas = 0;
        return true;
      } catch {
        return false;
      }
    },

    iniciar_sondeo: function () {
      var vm = this;
      vm.detener_sondeo();
      vm.refrescar_contador();
      temporizador = window.setInterval(() => vm.refrescar_contador(), INTERVALO_MS);
    },

    detener_sondeo: function () {
      if (temporizador) {
        window.clearInterval(temporizador);
        temporizador = null;
      }
    },

    // Al cerrar sesión: el próximo usuario no tiene que ver la bandeja del anterior.
    limpiar: function () {
      var vm = this;
      vm.notificaciones = [];
      vm.no_leidas = 0;
      vm.error = null;
    },
  },
});