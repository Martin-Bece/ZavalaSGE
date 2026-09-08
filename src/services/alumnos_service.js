import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_alumnos(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/alumnos${query}`, metodo: 'GET' });
}

export function obtener_alumno(id) {
	return ajax_request({ endpoint: `/api/alumnos/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_alumno(datos) {
	return ajax_request({ endpoint: '/api/alumnos', metodo: 'POST', datos });
}

export function actualizar_alumno(id, datos) {
	return ajax_request({ endpoint: `/api/alumnos/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_alumno(id) {
	return ajax_request({ endpoint: `/api/alumnos/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}