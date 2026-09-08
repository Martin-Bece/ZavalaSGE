import { defineStore } from 'pinia';
import {
	actualizar_materia,
	crear_materia,
	eliminar_materia,
	obtener_materias
} from '@/services/materias_service';

export const use_materias_store = defineStore('materias', {
	state: () => ({
		materias: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_materias(state) {
			return state.materias.length > 0;
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
				const respuesta = await obtener_materias({ ...vm.filtros, pagina: 1 });
				vm.materias = respuesta?.materias || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.materias;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las materias.';
				vm.materias = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.materias;
			vm.cargando = true;
			try {
				const respuesta = await obtener_materias({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.materias.map(m => m.id);
				vm.materias = [...vm.materias, ...(respuesta?.materias || []).filter(m => !conocidos.includes(m.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.materias;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más materias.';
				return vm.materias;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_materia(id, datos);
				else await crear_materia(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la materia.';
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
				await eliminar_materia(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la materia.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});