// src/stores/app_store.js
import { defineStore } from 'pinia';

const CLAVE_TEMA = 'zavala_tema';
const CLAVE_VIBRACION = 'zavalasge_vibracion'; // Usamos tu clave adaptada

export const use_app_store = defineStore('app', {
    state: () => ({
        tema: 'oscuro',
        vibracion: localStorage.getItem(CLAVE_VIBRACION) != 'no' // Arranca activada por defecto
    }),
    getters: {
        tema_oscuro(state) {
            return state.tema === 'oscuro';
        },
        vibracion_activa(state) {
            return state.vibracion;
        }
    },
    actions: {
        aplicar_tema_guardado() {
            const tema_guardado = localStorage.getItem(CLAVE_TEMA);
            this.tema = tema_guardado === 'claro' ? 'claro' : 'oscuro';
            this.aplicar_tema_actual();
        },
        alternar_tema() {
            this.tema = this.tema === 'oscuro' ? 'claro' : 'oscuro';
            localStorage.setItem(CLAVE_TEMA, this.tema);
            this.aplicar_tema_actual();
        },
        aplicar_tema_actual() {
            document.body.classList.toggle('dark', this.tema === 'oscuro');
        },
        // Nueva acción para la vibración
        alternar_vibracion() {
            var vm = this;
            vm.vibracion = !vm.vibracion;
            localStorage.setItem(CLAVE_VIBRACION, vm.vibracion ? 'si' : 'no');
        }
    }
});