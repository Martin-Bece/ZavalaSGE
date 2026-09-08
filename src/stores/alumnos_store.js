import { defineStore } from 'pinia';
import {
	actualizar_alumno,
	crear_alumno,
	eliminar_alumno,
	obtener_alumnos
} from '@/services/alumnos_service';

export const use_alumnos_store = defineStore('alumnos', {
	state: () => ({
		alumnos: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_alumnos(state) {
			return state.alumnos.length > 0;
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
				const respuesta = await obtener_alumnos({ ...vm.filtros, pagina: 1 });
				vm.alumnos = respuesta?.alumnos || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.alumnos;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar los alumnos.';
				vm.alumnos = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.alumnos;
			vm.cargando = true;
			try {
				const respuesta = await obtener_alumnos({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.alumnos.map(alumno => alumno.id);
				vm.alumnos = [...vm.alumnos, ...(respuesta?.alumnos || []).filter(alumno => !conocidos.includes(alumno.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.alumnos;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más alumnos.';
				return vm.alumnos;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_alumno(id, datos);
				else await crear_alumno(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar el alumno.';
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
				await eliminar_alumno(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar el alumno.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});