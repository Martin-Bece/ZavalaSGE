import { defineStore } from 'pinia';
import {
    actualizar_usuario,
    crear_usuario,
    eliminar_usuario,
    obtener_usuarios,
    subir_foto_perfil // <-- Importamos la nueva función
} from '@/services/usuarios_service';

export const use_usuarios_store = defineStore('usuarios', {
    state: () => ({
        usuarios: [],
        pagina: null,
        filtros: { busqueda: '' },
        cargando: false,
        guardando: false,
        error: null
    }),
    getters: {
        hay_usuarios(state) {
            return state.usuarios.length > 0;
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
                const respuesta = await obtener_usuarios({ ...vm.filtros, pagina: 1 });
                vm.usuarios = respuesta?.usuarios || [];
                vm.pagina = respuesta?.pagina || null;
                return vm.usuarios;
            } catch (error) {
                vm.error = error.mensaje || 'No se pudieron cargar los usuarios.';
                vm.usuarios = [];
                vm.pagina = null;
                return [];
            } finally {
                vm.cargando = false;
            }
        },
        cargar_mas: async function() {
            var vm = this;
            if (vm.cargando || !vm.hay_mas) return vm.usuarios;
            vm.cargando = true;
            try {
                const respuesta = await obtener_usuarios({ ...vm.filtros, pagina: (vm.pagina?.pagina || 1) + 1 });
                const conocidos = vm.usuarios.map(u => u.id);
                vm.usuarios = [...vm.usuarios, ...(respuesta?.usuarios || []).filter(u => !conocidos.includes(u.id))];
                vm.pagina = respuesta?.pagina || null;
                return vm.usuarios;
            } catch (error) {
                vm.error = error.mensaje || 'No se pudieron cargar más usuarios.';
                vm.usuarios = usuarios; // Corrección menor: dejar vm.usuarios
                return vm.usuarios;
            } finally {
                vm.cargando = false;
            }
        },
        guardar: async function(id, datos) {
            var vm = this;
            vm.guardando = true;
            vm.error = null;
            try {
                if (id) await actualizar_usuario(id, datos);
                else await crear_usuario(datos);
                await vm.cargar();
                return true;
            } catch (error) {
                vm.error = error.mensaje || 'No se pudo guardar el usuario.';
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
                await eliminar_usuario(id);
                await vm.cargar();
                return true;
            } catch (error) {
                vm.error = error.mensaje || 'No se pudo eliminar el usuario.';
                return false;
            } finally {
                vm.guardando = false;
            }
        },
        // NUEVA ACCIÓN PARA SUBIR LA FOTO DESDE EL STORE
        subir_foto: async function(formData) {
            var vm = this;
            vm.guardando = true;
            vm.error = null;
            try {
                const respuesta = await subir_foto_perfil(formData);
                await vm.cargar(); // Recargamos la lista para reflejar la nueva foto
                return respuesta;
            } catch (error) {
                vm.error = error.mensaje || 'No se pudo subir la foto de perfil.';
                throw error;
            } finally {
                vm.guardando = false;
            }
        }
    }
});