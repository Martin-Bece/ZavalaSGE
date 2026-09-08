import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_cursos(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/cursos${query}`, metodo: 'GET' });
}

export function obtener_curso(id) {
	return ajax_request({ endpoint: `/api/cursos/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_curso(datos) {
	return ajax_request({ endpoint: '/api/cursos', metodo: 'POST', datos });
}

export function actualizar_curso(id, datos) {
	return ajax_request({ endpoint: `/api/cursos/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_curso(id) {
	return ajax_request({ endpoint: `/api/cursos/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}