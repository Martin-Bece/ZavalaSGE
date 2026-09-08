import { defineStore } from 'pinia';
import {
	actualizar_docente,
	crear_docente,
	eliminar_docente,
	obtener_docentes
} from '@/services/docentes_service';

export const use_docentes_store = defineStore('docentes', {
	state: () => ({
		docentes: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_docentes(state) {
			return state.docentes.length > 0;
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
				const respuesta = await obtener_docentes({ ...vm.filtros, pagina: 1 });
				vm.docentes = respuesta?.docentes || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.docentes;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar los docentes.';
				vm.docentes = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.docentes;
			vm.cargando = true;
			try {
				const respuesta = await obtener_docentes({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.docentes.map(d => d.id);
				vm.docentes = [...vm.docentes, ...(respuesta?.docentes || []).filter(d => !conocidos.includes(d.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.docentes;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más docentes.';
				return vm.docentes;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_docente(id, datos);
				else await crear_docente(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar el docente.';
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
				await eliminar_docente(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar el docente.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});