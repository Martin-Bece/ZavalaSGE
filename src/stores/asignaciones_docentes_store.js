import { defineStore } from 'pinia';
import {
	actualizar_asignacion,
	crear_asignacion,
	eliminar_asignacion,
	obtener_asignaciones
} from '@/services/asignaciones_docentes_service';

export const use_asignaciones_docentes_store = defineStore('asignaciones_docentes', {
	state: () => ({
		asignaciones: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_asignaciones(state) {
			return state.asignaciones.length > 0;
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
				const respuesta = await obtener_asignaciones({ ...vm.filtros, pagina: 1 });
				vm.asignaciones = respuesta?.asignaciones || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.asignaciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las asignaciones.';
				vm.asignaciones = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.asignaciones;
			vm.cargando = true;
			try {
				const respuesta = await obtener_asignaciones({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.asignaciones.map(a => a.id);
				vm.asignaciones = [...vm.asignaciones, ...(respuesta?.asignaciones || []).filter(a => !conocidos.includes(a.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.asignaciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más asignaciones.';
				return vm.asignaciones;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_asignacion(id, datos);
				else await crear_asignacion(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la asignación.';
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
				await eliminar_asignacion(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la asignación.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});