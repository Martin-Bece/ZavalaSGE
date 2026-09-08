import { reactive } from 'vue';
import { obtener_resumen_personas } from '@/services/personas_service';

export const personas_store = reactive({
	cargando: false,
	error: '',
	personas: [],
	get hay_personas() {
		return this.personas.length > 0;
	}
});

export async function cargar_personas(){
	personas_store.cargando = true;
	personas_store.error = '';
	try{
		const respuesta = await obtener_resumen_personas();
		personas_store.personas = respuesta.personas || [];
	}catch(error){
		personas_store.personas = [];
		personas_store.error = error.mensaje;
	}finally{
		personas_store.cargando = false;
	}
}