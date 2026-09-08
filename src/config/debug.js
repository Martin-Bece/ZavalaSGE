const API_URL_DEBUG_PREDETERMINADA = 'http://localhost:5259';

export const debug_activado = import.meta.env.VITE_DEBUG_ACTIVADO !== 'false';

export function obtener_api_url(){
	const url = debug_activado
		? import.meta.env.VITE_API_URL_DEBUG || API_URL_DEBUG_PREDETERMINADA
		: import.meta.env.VITE_API_URL || '';
	return url.replace(/\/+$/, '');
}