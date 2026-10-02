import $ from "jquery";
import { obtener_api_url } from "@/config/debug";
import {
  cabecera_autorizacion,
  guardar_sesion,
  notificar_sesion_expirada,
  obtener_refresh_token,
} from "@/services/token_service";
import { vibrar_error } from "@/services/vibracion_service";

const api_url = obtener_api_url();

let renovacion_en_curso = null;

let vibracion_suspendida = false;

// Corre una llamada sin vibrar si falla (por ejemplo, la consulta del contador en segundo plano).
export async function sin_vibrar(llamada) {
  vibracion_suspendida = true;
  try {
    return await llamada();
  } finally {
    vibracion_suspendida = false;
  }
}

const SIN_RENOVACION = [
  "api/sesion/login",
  "api/sesion/refresh",
  "api/sesion/logout",
];

function construir_url(endpoint) {
  if (!api_url) throw new Error("No se configuro la URL de la API.");
  return `${api_url}/${String(endpoint).replace(/^\/+/, "")}`;
}

function normalizar_error(xhr, text_status, error_thrown) {
  const respuesta = xhr.responseJSON || null;

  if (!vibracion_suspendida) vibrar_error();

  if (text_status === "timeout") {
    return {
      estado_http: xhr.status || 0,
      codigo: "tiempo_espera_agotado",
      mensaje: "La solicitud tardó demasiado en responder. Intentá nuevamente.",
      respuesta,
    };
  }
  if (text_status === "abort") {
    return {
      estado_http: xhr.status || 0,
      codigo: "cancelado",
      mensaje: "La solicitud fue cancelada.",
      respuesta,
    };
  }
  if (!xhr.status) {
    return {
      estado_http: 0,
      codigo: "sin_conexion",
      mensaje:
        "No se pudo conectar con la API. Revisá que esté levantada y que la URL sea la correcta.",
      respuesta,
    };
  }

  if (xhr.status === 403) {
    return {
      estado_http: 403,
      codigo: respuesta?.codigo || "sin_permiso",
      mensaje:
        respuesta?.mensaje || "Tu rol no tiene permiso para esta acción.",
      respuesta,
    };
  }
  return {
    estado_http: xhr.status,
    codigo: respuesta?.codigo || text_status || "error_ajax",
    mensaje:
      respuesta?.mensaje ||
      error_thrown ||
      "No se pudo completar la solicitud.",
    respuesta,
  };
}

function renovar_sesion() {
  if (renovacion_en_curso) return renovacion_en_curso;
  renovacion_en_curso = (async function () {
    const refresh_token = await obtener_refresh_token();
    if (!refresh_token) return false;
    let respuesta;
    try {
      respuesta = await new Promise((resolve, reject) => {
        $.ajax({
          url: construir_url("api/sesion/refresh"),
          method: "POST",
          data: JSON.stringify({ refresh_token }),
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          timeout: 15000,
          headers: { Accept: "application/json" },
        })
          .done(resolve)
          .fail(reject);
      });
    } catch (xhr) {
      // Sin respuesta no hay rechazo del servidor: se corta sin tocar la sesion guardada.
      if (!xhr.status) throw normalizar_error(xhr, "error", "");
      return false;
    }
    const sesion = respuesta?.sesion || null;
    if (!sesion?.token) return false;
    await guardar_sesion(
      sesion.token,
      sesion.expira_en,
      sesion.refresh_token,
      sesion.refresh_expira_en,
    );
    return true;
  })().finally(() => {
    renovacion_en_curso = null;
  });
  return renovacion_en_curso;
}

async function manejar_401(permitir_renovar, reintentar) {
  const expirada = {
    estado_http: 401,
    codigo: "sesion_expirada",
    mensaje: "La sesión expiró. Volvé a iniciar sesión.",
    respuesta: null,
  };
  if (!permitir_renovar) {
    notificar_sesion_expirada();
    throw expirada;
  }
  const renovada = await renovar_sesion();
  if (!renovada) {
    notificar_sesion_expirada();
    throw expirada;
  }
  return reintentar();
}

function pedir(endpoint, metodo, datos, permitir_renovar) {
  const es_form_data = datos instanceof FormData;
  const envia_datos = datos !== undefined && datos !== null;

  return new Promise((resolve, reject) => {
    $.ajax({
      url: construir_url(endpoint),
      method: metodo,
      data: envia_datos
        ? es_form_data
          ? datos
          : JSON.stringify(datos)
        : undefined,
      dataType: "json",
      processData: es_form_data ? false : true,
      contentType: es_form_data
        ? false
        : envia_datos
        ? "application/json; charset=utf-8"
        : undefined,
      timeout: es_form_data ? 30000 : 15000,
      headers: { Accept: "application/json", ...cabecera_autorizacion() },
    })
      .done((respuesta) => resolve(respuesta))
      .fail((xhr, text_status, error_thrown) => {
        if (xhr.status === 401 && !SIN_RENOVACION.includes(endpoint)) {
          manejar_401(permitir_renovar, () =>
            pedir(endpoint, metodo, datos, false),
          ).then(resolve, (error) =>
            reject(
              error?.codigo
                ? error
                : normalizar_error(xhr, text_status, error_thrown),
            ),
          );
          return;
        }
        reject(normalizar_error(xhr, text_status, error_thrown));
      });
  });
}

// El error de la API también llega como Blob cuando pedimos un binario: hay que leerlo.
async function leer_json(blob) {
  try {
    if (!blob) return null;
    return JSON.parse(await blob.text());
  } catch {
    return null;
  }
}

function pedir_binario(endpoint, permitir_renovar) {
  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open("GET", construir_url(endpoint), true);
    xhr.responseType = "blob";
    xhr.timeout = 30000; // un PDF tarda más que un JSON

    const cabeceras = { Accept: "*/*", ...cabecera_autorizacion() };
    Object.keys(cabeceras).forEach((nombre) =>
      xhr.setRequestHeader(nombre, cabeceras[nombre]),
    );

    xhr.onload = async function () {
      if (xhr.status >= 200 && xhr.status < 300) {
        resolve(xhr.response);
        return;
      }
      if (xhr.status === 401 && !SIN_RENOVACION.includes(endpoint)) {
        try {
          resolve(
            await manejar_401(permitir_renovar, () =>
              pedir_binario(endpoint, false),
            ),
          );
        } catch (error) {
          reject(error);
        }
        return;
      }
      const respuesta = await leer_json(xhr.response);
      reject(
        normalizar_error(
          { status: xhr.status, responseJSON: respuesta },
          "error",
          "No se pudo descargar el archivo.",
        ),
      );
    };
    xhr.onerror = () =>
      reject(normalizar_error({ status: 0, responseJSON: null }, "error", ""));
    xhr.ontimeout = () =>
      reject(
        normalizar_error({ status: 0, responseJSON: null }, "timeout", ""),
      );

    xhr.send();
  });
}

export function ajax_binario(endpoint) {
  return pedir_binario(endpoint, true);
}

export function obtener(endpoint) {
  return pedir(endpoint, "GET", undefined, true);
}

// La escritura: POST, PUT y DELETE. El cuerpo viaja como JSON en snake_case, igual que llega:
// la misma convencion de punta a punta, tambien de ida.
export function enviar(endpoint, metodo, datos) {
  return pedir(endpoint, metodo, datos, true);
}

export function ajax_request(configuracion = {}) {
  const { endpoint, metodo = "GET", datos = null } = configuracion;
  if (metodo.toUpperCase() === "GET") {
    return obtener(endpoint);
  }
  return enviar(endpoint, metodo, datos);
}

export function enviar_archivo(endpoint, metodo, form_data) {
  return new Promise((resolve, reject) => {
    $.ajax({
      url: construir_url(endpoint),
      method: metodo,
      data: form_data,
      processData: false, // Evita que jQuery procese el FormData
      contentType: false, // Evita que jQuery ponga un contentType por defecto
      timeout: 30000, // Un poco más de tiempo por si la imagen es pesada
      headers: { Accept: "application/json", ...cabecera_autorizacion() },
    })
      .done((respuesta) => resolve(respuesta))
      .fail((xhr, text_status, error_thrown) => {
        // Opcional: Podrías manejar el 401 aquí también si tu perfil requiere renovación de token
        reject(normalizar_error(xhr, text_status, error_thrown));
      });
  });
}
