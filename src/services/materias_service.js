import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_materias(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/materias${query}`, metodo: 'GET' });
}

export function obtener_materia(id) {
	return ajax_request({ endpoint: `/api/materias/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_materia(datos) {
	return ajax_request({ endpoint: '/api/materias', metodo: 'POST', datos });
}

export function actualizar_materia(id, datos) {
	return ajax_request({ endpoint: `/api/materias/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_materia(id) {
	return ajax_request({ endpoint: `/api/materias/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}