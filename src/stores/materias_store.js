import { reactive } from 'vue';
import { obtener_resumen_materias } from '@/services/materias_service';

export const materias_store = reactive({
	cargando: false,
	error: '',
	materias: [],
	get hay_materias() {
		return this.materias.length > 0;
	}
});

export async function cargar_materias(){
	materias_store.cargando = true;
	materias_store.error = '';
	try{
		const respuesta = await obtener_resumen_materias();
		materias_store.materias = respuesta.materias || [];
	}catch(error){
		materias_store.materias = [];
		materias_store.error = error.mensaje;
	}finally{
		materias_store.cargando = false;
	}
}