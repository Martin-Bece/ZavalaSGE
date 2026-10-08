import { Capacitor } from "@capacitor/core";
import { PushNotifications } from "@capacitor/push-notifications";

// El push viene prendido: apagarlo es una bandera del .env (VITE_PUSH_ACTIVO=false), no un olvido.
const PUSH_ACTIVO = import.meta.env.VITE_PUSH_ACTIVO !== "false";

let iniciado = false;

export function push_disponible() {
  return PUSH_ACTIVO && Capacitor.isNativePlatform();
}

// Registra el teléfono en Firebase y escucha los avisos. Devuelve {ok, mensaje}.
// mensaje null = no hay nada que mostrar (éxito, navegador o push apagado).
export async function iniciar_push({ al_recibir, al_tocar } = {}) {
  if (!push_disponible()) return { ok: false, mensaje: null };
  if (iniciado) return { ok: true, mensaje: null };

  try {
    let permisos = await PushNotifications.checkPermissions();
    if (permisos.receive === "prompt" || permisos.receive === "prompt-with-rationale") {
      permisos = await PushNotifications.requestPermissions();
    }
    if (permisos.receive !== "granted") {
      return { ok: false, mensaje: "Las notificaciones push están desactivadas para la app." };
    }

    // La app solo se registra y recibe: el mensaje lo entrega Firebase, no nuestra API.
    await PushNotifications.addListener("registration", (token) => {
      console.log("token FCM", token.value);
    });
    await PushNotifications.addListener("registrationError", (error) => {
      console.warn("registro push fallido", JSON.stringify(error));
    });
    // Con la app abierta.
    await PushNotifications.addListener("pushNotificationReceived", (notificacion) => {
      if (al_recibir) al_recibir(notificacion);
    });
    // Cuando el usuario toca el aviso.
    await PushNotifications.addListener("pushNotificationActionPerformed", (accion) => {
      if (al_tocar) al_tocar(accion.notification);
    });

    await PushNotifications.register();
    iniciado = true;
    return { ok: true, mensaje: null };
  } catch (error) {
    // Sin google-services.json el registro falla: se agarra acá y todo lo demás sigue andando.
    return { ok: false, mensaje: (error && error.message) || "No se pudo activar el push." };
  }
}

export async function detener_push() {
  if (!iniciado) return;
  try {
    await PushNotifications.removeAllListeners();
  } catch {
    // nada que hacer
  }
  iniciado = false;
}