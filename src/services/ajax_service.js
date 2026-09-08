import $ from 'jquery';
import { obtener_api_url } from '@/config/debug';

const api_url = obtener_api_url();

function construir_url(endpoint){
    if(!api_url) throw new Error('No se configuro la URL de la API.');
    return `${api_url}/${String(endpoint).replace(/^\/+/, '')}`;
}

function normalizar_error(xhr, text_status, error_thrown){
    const respuesta = xhr.responseJSON || null;
    if(text_status === 'timeout'){
        return {
            estado_http: xhr.status || 0,
            codigo: 'tiempo_espera_agotado',
            mensaje: 'La solicitud tardó demasiado en responder. Intenta nuevamente.',
            respuesta
        };
    }
    if(text_status === 'abort'){
        return {
            estado_http: xhr.status || 0,
            codigo: 'cancelado',
            mensaje: 'La solicitud fue cancelada.',
            respuesta
        };
    }
    if(!xhr.status){
        return {
            estado_http: 0,
            codigo: 'sin_conexion',
            mensaje: 'No se pudo conectar con la API. Revisá que esté levantada y que la URL sea la correcta.',
            respuesta
        };
    }
    return {
        estado_http: xhr.status,
        codigo: respuesta?.codigo || text_status || 'error_ajax',
        mensaje: respuesta?.mensaje || error_thrown || 'No se pudo completar la solicitud.',
        respuesta
    };
}

export function ajax_request(configuracion = {}){
    const {
        endpoint,
        metodo = 'GET',
        datos = null,
        timeout = 15000,
        headers = {}
    } = configuracion;
    const metodo_normalizado = metodo.toUpperCase();
    const envia_json = datos !== null && !['GET', 'HEAD'].includes(metodo_normalizado);
    return new Promise((resolve, reject) =>{
        $.ajax({
            url: construir_url(endpoint),
            method: metodo_normalizado,
            data: envia_json ? JSON.stringify(datos) : datos,
            dataType: 'json',
            contentType: envia_json ? 'application/json; charset=utf-8' : undefined,
            timeout,
            headers: {
                Accept: 'application/json',
                ...headers
            }
        })
            .done(respuesta => resolve(respuesta))
            .fail((xhr, text_status, error_thrown) => reject(normalizar_error(xhr, text_status, error_thrown)));
    });
}

// FUNCIONES DE CONVENIENCIA EXPORTADAS
export function obtener(endpoint, datos = null, opciones = {}) {
    return ajax_request({ endpoint, metodo: 'GET', datos, ...opciones });
}

export function publicar(endpoint, datos = null, opciones = {}) {
    return ajax_request({ endpoint, metodo: 'POST', datos, ...opciones });
}

export function actualizar(endpoint, datos = null, opciones = {}) {
    return ajax_request({ endpoint, metodo: 'PUT', datos, ...opciones });
}

export function eliminar(endpoint, opciones = {}) {
    return ajax_request({ endpoint, metodo: 'DELETE', ...opciones });
}