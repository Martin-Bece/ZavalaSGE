import { enviar, obtener } from "@/services/ajax_service";

export function obtener_notificaciones() {
  return obtener("api/notificaciones");
}

// Solo el número, para el globo de la campana.
export function obtener_no_leidas() {
  return obtener("api/notificaciones/no-leidas");
}

export function marcar_leida(id) {
  return enviar(`api/notificaciones/${id}/leida`, "PUT");
}

export function marcar_todas_leidas() {
  return enviar("api/notificaciones/leidas", "PUT");
}