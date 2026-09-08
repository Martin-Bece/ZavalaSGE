import { obtener } from '@/services/ajax_service';

export function obtener_resumen_materias(){
	return obtener('api/materias');
}

export function obtener_materia(id){
	return obtener(`api/materias/${id}`);
}