import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_docentes(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/docentes${query}`, metodo: 'GET' });
}

export function obtener_docente(id) {
	return ajax_request({ endpoint: `/api/docentes/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_docente(datos) {
	return ajax_request({ endpoint: '/api/docentes', metodo: 'POST', datos });
}

export function actualizar_docente(id, datos) {
	return ajax_request({ endpoint: `/api/docentes/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_docente(id) {
	return ajax_request({ endpoint: `/api/docentes/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}