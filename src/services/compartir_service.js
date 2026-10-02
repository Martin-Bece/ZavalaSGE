import { Capacitor } from "@capacitor/core";
import { Directory, Filesystem } from "@capacitor/filesystem";
import { Share } from "@capacitor/share";

// Devuelve {ok, mensaje}. mensaje null = no hay nada que mostrar (éxito o cancelación).
export async function compartir_archivo({ nombre_archivo, blob, titulo, texto }) {
  if (!blob) return { ok: false, mensaje: "No hay archivo para compartir." };
  try {
    if (!Capacitor.isNativePlatform()) {
      return await compartir_archivo_web({ nombre_archivo, blob, titulo, texto });
    }
    const base64 = await blob_a_base64(blob);
    const escrito = await Filesystem.writeFile({
      path: nombre_archivo,
      data: base64,
      // Cache y no Documents: es privada de la app (la constancia lleva nombre y DNI).
      directory: Directory.Cache,
    });
    await Share.share({
      title: titulo,
      text: texto,
      files: [escrito.uri],
      dialogTitle: titulo || "Compartir constancia",
    });
    return { ok: true, mensaje: null };
  } catch (error) {
    return { ok: false, mensaje: normalizar_mensaje(error) };
  }
}

// En el navegador: diálogo de compartir si existe; si no, descarga directa.
async function compartir_archivo_web({ nombre_archivo, blob, titulo, texto }) {
  const archivo = new File([blob], nombre_archivo, { type: blob.type });
  if (navigator.canShare && navigator.canShare({ files: [archivo] })) {
    await navigator.share({ files: [archivo], title: titulo, text: texto });
    return { ok: true, mensaje: null };
  }
  const url = URL.createObjectURL(blob);
  const enlace = document.createElement("a");
  enlace.href = url;
  enlace.download = nombre_archivo;
  enlace.click();
  URL.revokeObjectURL(url);
  return { ok: true, mensaje: null };
}

// Filesystem.writeFile espera el contenido en base64, sin el prefijo "data:...;base64,".
function blob_a_base64(blob) {
  return new Promise((resolve, reject) => {
    const lector = new FileReader();
    lector.onload = () => resolve(String(lector.result).split(",")[1]);
    lector.onerror = () => reject(new Error("No se pudo leer el archivo."));
    lector.readAsDataURL(blob);
  });
}

function normalizar_mensaje(error) {
  const mensaje = (error && error.message) || "";
  // Cerrar el diálogo de compartir sin elegir nada no es un error que mostrar.
  if (/cancel|abort/i.test(mensaje) || (error && error.name === "AbortError")) return null;
  return mensaje || "No se pudo compartir el archivo.";
}