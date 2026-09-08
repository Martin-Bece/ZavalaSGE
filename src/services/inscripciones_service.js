import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_inscripciones(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/inscripciones${query}`, metodo: 'GET' });
}

export function obtener_inscripcion(id) {
	return ajax_request({ endpoint: `/api/inscripciones/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_inscripcion(datos) {
	return ajax_request({ endpoint: '/api/inscripciones', metodo: 'POST', datos });
}

export function actualizar_inscripcion(id, datos) {
	return ajax_request({ endpoint: `/api/inscripciones/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_inscripcion(id) {
	return ajax_request({ endpoint: `/api/inscripciones/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}