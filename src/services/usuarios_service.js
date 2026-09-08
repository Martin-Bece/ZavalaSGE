import { ajax_request } from './ajax_service';
import { construir_query } from '@/utils/consulta';

export function obtener_usuarios(opciones = {}) {
	const query = construir_query({
		busqueda: opciones.busqueda,
		pagina: opciones.pagina,
		tamano: opciones.tamano
	});
	return ajax_request({ endpoint: `/api/usuarios${query}`, metodo: 'GET' });
}

export function obtener_usuario(id) {
	return ajax_request({ endpoint: `/api/usuarios/${encodeURIComponent(id)}`, metodo: 'GET' });
}

export function crear_usuario(datos) {
	return ajax_request({ endpoint: '/api/usuarios', metodo: 'POST', datos });
}

export function actualizar_usuario(id, datos) {
	return ajax_request({ endpoint: `/api/usuarios/${encodeURIComponent(id)}`, metodo: 'PUT', datos });
}

export function eliminar_usuario(id) {
	return ajax_request({ endpoint: `/api/usuarios/${encodeURIComponent(id)}`, metodo: 'DELETE' });
}