import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_comisiones(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/comisiones${query}`, metodo: 'GET' });
}

export function obtener_comision(id) {
	return ajax_request({ endpoint: `/api/comisiones/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_comision(datos) {
	return ajax_request({ endpoint: '/api/comisiones', metodo: 'POST', datos });
}

export function actualizar_comision(id, datos) {
	return ajax_request({ endpoint: `/api/comisiones/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_comision(id) {
	return ajax_request({ endpoint: `/api/comisiones/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}