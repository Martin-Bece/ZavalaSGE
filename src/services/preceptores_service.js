import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_preceptores(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/preceptores${query}`, metodo: 'GET' });
}

export function obtener_preceptor(id) {
	return ajax_request({ endpoint: `/api/preceptores/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_preceptor(datos) {
	return ajax_request({ endpoint: '/api/preceptores', metodo: 'POST', datos });
}

export function actualizar_preceptor(id, datos) {
	return ajax_request({ endpoint: `/api/preceptores/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_preceptor(id) {
	return ajax_request({ endpoint: `/api/preceptores/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}