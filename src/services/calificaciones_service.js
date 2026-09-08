import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_calificaciones(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/calificaciones${query}`, metodo: 'GET' });
}

export function obtener_calificacion(id) {
	return ajax_request({ endpoint: `/api/calificaciones/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_calificacion(datos) {
	return ajax_request({ endpoint: '/api/calificaciones', metodo: 'POST', datos });
}

export function actualizar_calificacion(id, datos) {
	return ajax_request({ endpoint: `/api/calificaciones/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_calificacion(id) {
	return ajax_request({ endpoint: `/api/calificaciones/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}