import { defineStore } from 'pinia';
import {
	actualizar_calificacion,
	crear_calificacion,
	eliminar_calificacion,
	obtener_calificaciones
} from '@/services/calificaciones_service';

export const use_calificaciones_store = defineStore('calificaciones', {
	state: () => ({
		calificaciones: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_calificaciones(state) {
			return state.calificaciones.length > 0;
		},
		hay_mas(state) {
			return Boolean(state.pagina?.hay_mas);
		},
		total(state) {
			return state.pagina?.total || 0;
		}
	},
	actions: {
		cargar: async function(filtros = null) {
			var vm = this;
			vm.cargando = true;
			vm.error = null;
			if (filtros) vm.filtros = { ...vm.filtros, ...filtros };
			try {
				const respuesta = await obtener_calificaciones({ ...vm.filtros, pagina: 1 });
				vm.calificaciones = respuesta?.calificaciones || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.calificaciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las calificaciones.';
				vm.calificaciones = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.calificaciones;
			vm.cargando = true;
			try {
				const respuesta = await obtener_calificaciones({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.calificaciones.map(c => c.id);
				vm.calificaciones = [...vm.calificaciones, ...(respuesta?.calificaciones || []).filter(c => !conocidos.includes(c.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.calificaciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más calificaciones.';
				return vm.calificaciones;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_calificacion(id, datos);
				else await crear_calificacion(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la calificación.';
				return false;
			} finally {
				vm.guardando = false;
			}
		},
		eliminar: async function(id) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				await eliminar_calificacion(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la calificación.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});