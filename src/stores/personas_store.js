import { defineStore } from 'pinia';
import {
	actualizar_persona,
	crear_persona,
	eliminar_persona,
	obtener_personas
} from '@/services/personas_service';

export const use_personas_store = defineStore('personas', {
	state: () => ({
		personas: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_personas(state) {
			return state.personas.length > 0;
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
				const respuesta = await obtener_personas({ ...vm.filtros, pagina: 1 });
				vm.personas = respuesta?.personas || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.personas;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las personas.';
				vm.personas = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.personas;
			vm.cargando = true;
			try {
				const respuesta = await obtener_personas({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.personas.map(p => p.id);
				vm.personas = [...vm.personas, ...(respuesta?.personas || []).filter(p => !conocidos.includes(p.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.personas;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más personas.';
				return vm.personas;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_persona(id, datos);
				else await crear_persona(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la persona.';
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
				await eliminar_persona(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la persona.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});