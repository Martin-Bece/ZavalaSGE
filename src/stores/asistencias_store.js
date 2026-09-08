import { defineStore } from 'pinia';
import {
	actualizar_asistencia,
	crear_asistencia,
	eliminar_asistencia,
	obtener_asistencias
} from '@/services/asistencias_service';

export const use_asistencias_store = defineStore('asistencias', {
	state: () => ({
		asistencias: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_asistencias(state) {
			return state.asistencias.length > 0;
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
				const respuesta = await obtener_asistencias({ ...vm.filtros, pagina: 1 });
				vm.asistencias = respuesta?.asistencias || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.asistencias;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las asistencias.';
				vm.asistencias = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.asistencias;
			vm.cargando = true;
			try {
				const respuesta = await obtener_asistencias({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.asistencias.map(a => a.id);
				vm.asistencias = [...vm.asistencias, ...(respuesta?.asistencias || []).filter(a => !conocidos.includes(a.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.asistencias;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más asistencias.';
				return vm.asistencias;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_asistencia(id, datos);
				else await crear_asistencia(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la asistencia.';
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
				await eliminar_asistencia(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la asistencia.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});