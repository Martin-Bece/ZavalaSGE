import { defineStore } from 'pinia';
import {
	actualizar_curso,
	crear_curso,
	eliminar_curso,
	obtener_cursos
} from '@/services/cursos_service';

export const use_cursos_store = defineStore('cursos', {
	state: () => ({
		cursos: [],
		pagina: null,
		filtros: { busqueda: '' },
		cargando: false,
		guardando: false,
		error: null
	}),
	getters: {
		hay_cursos(state) {
			return state.cursos.length > 0;
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
				const respuesta = await obtener_cursos({ ...vm.filtros, pagina: 1 });
				vm.cursos = respuesta?.cursos || [];
				vm.pagina = respuesta?.pagina || null;
				return vm.cursos;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar los cursos.';
				vm.cursos = [];
				vm.pagina = null;
				return [];
			} finally {
				vm.cargando = false;
			}
		},
		cargar_mas: async function() {
			var vm = this;
			if (vm.cargando || !vm.hay_mas) return vm.cursos;
			vm.cargando = true;
			try {
				const respuesta = await obtener_cursos({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
				const conocidos = vm.cursos.map(c => c.id);
				vm.cursos = [...vm.cursos, ...(respuesta?.cursos || []).filter(c => !conocidos.includes(c.id))];
				vm.pagina = respuesta?.pagina || null;
				return vm.cursos;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudieron cargar más cursos.';
				return vm.cursos;
			} finally {
				vm.cargando = false;
			}
		},
		guardar: async function(id, datos) {
			var vm = this;
			vm.guardando = true;
			vm.error = null;
			try {
				if (id) await actualizar_curso(id, datos);
				else await crear_curso(datos);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo guardar el curso.';
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
				await eliminar_curso(id);
				await vm.cargar();
				return true;
			} catch (error) {
				vm.error = error.mensaje || 'No se pudo eliminar el curso.';
				return false;
			} finally {
				vm.guardando = false;
			}
		}
	}
});