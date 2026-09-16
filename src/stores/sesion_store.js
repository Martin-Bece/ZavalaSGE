import { defineStore } from "pinia";
import {
  cerrar_sesion,
  iniciar_sesion,
  obtener_mi_usuario,
} from "@/services/sesion_service";
import {
  borrar_sesion,
  guardar_sesion,
  obtener_refresh_token,
  restaurar_sesion,
} from "@/services/token_service";

export const use_sesion_store = defineStore("sesion", {
  state: () => ({
    cargando: false,
    error: null,
    restaurando: true,
    usuario: null,
  }),
  getters: {
    autenticado(state) {
      return Boolean(state.usuario);
    },
    rol_activo(state) {
      // Mapea la propiedad exacta 'rolCodigo' o 'rolId' devuelta por la API
      const r =
        state.usuario?.rolCodigo ||
        state.usuario?.rolId ||
        state.usuario?.rol_codigo ||
        state.usuario?.rol ||
        state.usuario?.Rol ||
        null;

      return typeof r === "string" ? r.toUpperCase() : null;
    },
    pendiente_de_habilitacion(state) {
      return Boolean(state.usuario) && !this.rol_activo;
    },
    es_admin() {
      return this.rol_activo === "ADMIN";
    },
  },
  actions: {
    preparar: async function () {
      var vm = this;
      vm.restaurando = true;
      try {
        return Boolean(await restaurar_sesion());
      } finally {
        vm.restaurando = false;
      }
    },

    iniciar: async function () {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      try {
        const token = await restaurar_sesion();
        if (!token) {
          vm.usuario = null;
          return null;
        }
        const respuesta = await obtener_mi_usuario();
        vm.usuario = respuesta?.usuario || null;
        return vm.usuario;
      } catch (error) {
        const revocada = error?.estado_http === 401;
        if (revocada) await borrar_sesion();

        vm.error = revocada
          ? "La sesión guardada ya no vale. Entrá con tu email y contraseña."
          : error?.mensaje || "No se pudo reanudar la sesión.";
        vm.usuario = null;
        return null;
      } finally {
        vm.cargando = false;
      }
    },
    entrar: async function (email, password) {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      try {
        const respuesta = await iniciar_sesion(email, password);
        const sesion = respuesta?.sesion || null;
        if (!sesion?.token) {
          vm.error = "No se pudo iniciar sesión.";
          return false;
        }
        await guardar_sesion(
          sesion.token,
          sesion.expira_en,
          sesion.refresh_token,
          sesion.refresh_expira_en,
        );
        vm.usuario = sesion.usuario;
        return true;
      } catch (error) {
        vm.error = error.mensaje || "No se pudo iniciar sesión.";
        return false;
      } finally {
        vm.cargando = false;
      }
    },
    salir: async function (todos = false) {
      var vm = this;
      try {
        const refresh_token = await obtener_refresh_token();

        if (refresh_token) await cerrar_sesion(refresh_token, todos);
      } catch {}
      await vm.limpiar();
    },
    limpiar: async function () {
      var vm = this;
      await borrar_sesion();
      vm.usuario = null;
      vm.error = null;
    },
  },
});
