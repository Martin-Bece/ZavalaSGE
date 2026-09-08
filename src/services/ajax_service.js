import { obtener_api_url } from '@/config/debug';

const api_url = obtener_api_url();

function construir_url(endpoint){
	if(!api_url) throw new Error('No se configuro la URL de la API.');
	return `${api_url}/${String(endpoint).replace(/^\/+/, '')}`;
}

function error_normalizado(estado_http, codigo, mensaje){
	return {estado_http, codigo, mensaje};
}

export async function obtener(endpoint){
	let respuesta;
	try{
		respuesta = await window.fetch(construir_url(endpoint), {headers: {Accept: 'application/json'}});
	}catch{
		throw error_normalizado(0, 'sin_conexion', 'No se pudo conectar con la API. Revisá que esté levantada y que la URL sea la correcta.');
	}
	if(!respuesta.ok){
		const cuerpo = await respuesta.json().catch(() => null);
		throw error_normalizado(respuesta.status, cuerpo?.codigo || 'error_http', cuerpo?.mensaje || `La API respondió ${respuesta.status}.`);
	}
	return await respuesta.json();
}