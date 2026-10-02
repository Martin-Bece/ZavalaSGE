import { enviar, obtener, ajax_binario } from "@/services/ajax_service";

// Preceptor: sus comisiones.
export function obtener_comisiones_preceptor() {
  return obtener("api/preceptor/comisiones");
}

// Docente: sus asignaciones (materia + comisión).
export function obtener_materias_docente() {
  return obtener("api/docente/materias");
}

// Alumnos de una comisión. La API controla que sea tuya.
export function obtener_alumnos_comision(comision_id) {
  return obtener(`api/comisiones/${comision_id}/alumnos`);
}

// Alumnos de una comisión con sus notas. El docente debe indicar la materia;
// preceptor y admin pueden omitirla para ver todas.
export function obtener_calificaciones_comision(comision_id, materia_id = null) {
  const query = materia_id ? `?materiaId=${materia_id}` : "";
  return obtener(`api/comisiones/${comision_id}/calificaciones${query}`);
}

export function crear_calificacion(datos) {
  return enviar("api/calificaciones", "POST", datos);
}

export function actualizar_calificacion(id, datos) {
  return enviar(`api/calificaciones/${id}`, "PUT", datos);
}

export function eliminar_calificacion(id) {
  return enviar(`api/calificaciones/${id}`, "DELETE");
}

export function obtener_ficha_alumno(id) {
  return obtener(`api/alumnos/${id}`);
}

// Solo lo propio del alumno (legajo y estado). Los datos de la persona no se tocan.
export function actualizar_datos_alumno(id, datos) {
  return enviar(`api/alumnos/${id}/datos`, "PUT", datos);
}

// La constancia es un PDF: viaja como binario (Blob), no como JSON.
export function descargar_constancia_alumno(id) {
  return ajax_binario(`api/alumnos/${id}/constancia`);
}