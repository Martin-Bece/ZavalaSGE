import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_asistencias(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/asistencias${query}`, metodo: 'GET' });
}

export function obtener_asistencia(id) {
	return ajax_request({ endpoint: `/api/asistencias/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_asistencia(datos) {
	return ajax_request({ endpoint: '/api/asistencias', metodo: 'POST', datos });
}

export function actualizar_asistencia(id, datos) {
	return ajax_request({ endpoint: `/api/asistencias/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_asistencia(id) {
	return ajax_request({ endpoint: `/api/asistencias/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}