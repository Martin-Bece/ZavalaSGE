import { defineStore } from 'pinia';
import {
	actualizar_inscripcion,
	crear_inscripcion,
	eliminar_inscripcion,
	obtener_inscripciones
} from '@/services/inscripciones_service';

export const use_inscripciones_store = defineStore('inscripciones', {
	state: () => ({
		inscripciones: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_inscripciones(state) {
			return state.inscripciones.length > 0;
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
				const respuesta = await obtener_inscripciones({ ...vm.filtros, pagina: 1 });
				vm.inscripciones = respuesta?.inscripciones || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.inscripciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las inscripciones.';
				vm.inscripciones = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.inscripciones;
			vm.cargando = true;
			try {
				const respuesta = await obtener_inscripciones({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.inscripciones.map(i => i.id);
				vm.inscripciones = [...vm.inscripciones, ...(respuesta?.inscripciones || []).filter(i => !conocidos.includes(i.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.inscripciones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más inscripciones.';
				return vm.inscripciones;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_inscripcion(id, datos);
				else await crear_inscripcion(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la inscripción.';
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
				await eliminar_inscripcion(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la inscripción.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});