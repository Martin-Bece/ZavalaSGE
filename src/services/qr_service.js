import { BarcodeFormat, BarcodeScanner } from "@capacitor-mlkit/barcode-scanning";
import { Capacitor } from "@capacitor/core";

// El escáner solo existe en la app instalada. En el navegador se tipea el código.
export function escaner_disponible() {
  return Capacitor.isNativePlatform();
}

// 'ALU-0007' pasa a 7. Cualquier otro texto no es la constancia de un alumno: null.
export function id_de_alumno_desde_codigo(texto) {
  const coincidencia = /^ALU-(\d+)$/i.exec(String(texto || "").trim());
  return coincidencia ? Number(coincidencia[1]) : null;
}

// Devuelve {ok, contenido, mensaje}. mensaje null = no hay nada que mostrar (éxito o cancelación).
export async function escanear_qr() {
  try {
    const soporte = await BarcodeScanner.isSupported();
    if (!soporte.supported) {
      return { ok: false, contenido: null, mensaje: "Este dispositivo no puede escanear códigos." };
    }

    // La primera vez Android descarga el lector de Google aparte: es otro final abierto, como el permiso.
    const modulo = await BarcodeScanner.isGoogleBarcodeScannerModuleAvailable();
    if (!modulo.available) {
      await BarcodeScanner.installGoogleBarcodeScannerModule();
      return {
        ok: false,
        contenido: null,
        mensaje: "Se está descargando el lector de códigos de Google. Reintentá en unos segundos.",
      };
    }

    const resultado = await BarcodeScanner.scan({ formats: [BarcodeFormat.QrCode] });
    const contenido =
      (resultado && resultado.barcodes && resultado.barcodes[0] && resultado.barcodes[0].rawValue) || "";
    if (!contenido) return { ok: false, contenido: null, mensaje: "No se leyó ningún código." };
    return { ok: true, contenido, mensaje: null };
  } catch (error) {
    // Cerrar el lector sin leer nada no es un error que mostrar.
    if (/cancel/i.test(String((error && error.message) || ""))) {
      return { ok: false, contenido: null, mensaje: null };
    }
    return {
      ok: false,
      contenido: null,
      mensaje: (error && error.message) || "No se pudo abrir el escáner.",
    };
  }
}