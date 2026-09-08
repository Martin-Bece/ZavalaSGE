import { defineStore } from 'pinia';
import {
	actualizar_preceptor,
	crear_preceptor,
	eliminar_preceptor,
	obtener_preceptores
} from '@/services/preceptores_service';

export const use_preceptores_store = defineStore('preceptores', {
	state: () => ({
		preceptores: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_preceptores(state) {
			return state.preceptores.length > 0;
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
				const respuesta = await obtener_preceptores({ ...vm.filtros, pagina: 1 });
				vm.preceptores = respuesta?.preceptores || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.preceptores;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar los preceptores.';
				vm.preceptores = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.preceptores;
			vm.cargando = true;
			try {
				const respuesta = await obtener_preceptores({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.preceptores.map(p => p.id);
				vm.preceptores = [...vm.preceptores, ...(respuesta?.preceptores || []).filter(p => !conocidos.includes(p.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.preceptores;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más preceptores.';
				return vm.preceptores;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_preceptor(id, datos);
				else await crear_preceptor(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar el preceptor.';
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
				await eliminar_preceptor(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar el preceptor.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});