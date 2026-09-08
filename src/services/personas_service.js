import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_personas(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/personas${query}`, metodo: 'GET' });
}

export function obtener_persona(id) {
	return ajax_request({ endpoint: `/api/personas/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_persona(datos) {
	return ajax_request({ endpoint: '/api/personas', metodo: 'POST', datos });
}

export function actualizar_persona(id, datos) {
	return ajax_request({ endpoint: `/api/personas/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_persona(id) {
	return ajax_request({ endpoint: `/api/personas/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}