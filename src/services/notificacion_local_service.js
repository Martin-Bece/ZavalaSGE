import { Capacitor } from "@capacitor/core";
import { LocalNotifications } from "@capacitor/local-notifications";

// Devuelve {ok, mensaje}. mensaje null = no hay nada que mostrar (éxito, o navegador).
export async function avisar_local(titulo, mensaje) {
  // En el navegador no hay notificación del sistema, y eso no es una falla que contar.
  if (!Capacitor.isNativePlatform()) return { ok: false, mensaje: null };

  try {
    let permisos = await LocalNotifications.checkPermissions();
    if (permisos.display !== "granted") {
      permisos = await LocalNotifications.requestPermissions();
    }
    if (permisos.display !== "granted") {
      return {
        ok: false,
        mensaje: "Las notificaciones del teléfono están desactivadas para la app.",
      };
    }

    await LocalNotifications.schedule({
      notifications: [
        {
          // el id es obligatorio y tiene tope de int32
          id: Date.now() % 2147483647,
          title: titulo,
          body: mensaje,
          schedule: { at: new Date(Date.now() + 250) },
        },
      ],
    });
    return { ok: true, mensaje: null };
  } catch (error) {
    return {
      ok: false,
      mensaje: (error && error.message) || "No se pudo mostrar la notificación.",
    };
  }
}