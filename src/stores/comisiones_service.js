import { defineStore } from 'pinia';
import {
	actualizar_comision,
	crear_comision,
	eliminar_comision,
	obtener_comisiones
} from '@/services/comisiones_service';

export const use_comisiones_store = defineStore('comisiones', {
	state: () => ({
		comisiones: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_comisiones(state) {
			return state.comisiones.length > 0;
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
				const respuesta = await obtener_comisiones({ ...vm.filtros, pagina: 1 });
				vm.comisiones = respuesta?.comisiones || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.comisiones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar las comisiones.';
				vm.comisiones = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.comisiones;
			vm.cargando = true;
			try {
				const respuesta = await obtener_comisiones({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.comisiones.map(c => c.id);
				vm.comisiones = [...vm.comisiones, ...(respuesta?.comisiones || []).filter(c => !conocidos.includes(c.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.comisiones;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más comisiones.';
				return vm.comisiones;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_comision(id, datos);
				else await crear_comision(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar la comisión.';
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
				await eliminar_comision(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar la comisión.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});