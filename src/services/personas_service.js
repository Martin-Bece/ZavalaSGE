import { obtener } from '@/services/ajax_service';

export function obtener_resumen_personas(){
	return obtener('api/personas');
}

export function obtener_persona(id){
	return obtener(`api/personas/${id}`);
}