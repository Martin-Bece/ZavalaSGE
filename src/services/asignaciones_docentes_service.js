import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_asignaciones(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/asignaciones-docentes${query}`, metodo: 'GET' });
}

export function obtener_asignacion(id) {
	return ajax_request({ endpoint: `/api/asignaciones-docentes/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_asignacion(datos) {
	return ajax_request({ endpoint: '/api/asignaciones-docentes', metodo: 'POST', datos });
}

export function actualizar_asignacion(id, datos) {
	return ajax_request({ endpoint: `/api/asignaciones-docentes/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_asignacion(id) {
	return ajax_request({ endpoint: `/api/asignaciones-docentes/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}