// src/services/vibracion_service.js
import { Haptics, ImpactStyle, NotificationType } from '@capacitor/haptics';
import { use_app_store } from '@/stores/app_store';

function vibracion_apagada() {
    return !use_app_store().vibracion_activa;
}

export async function vibrar_toque() {
    if (vibracion_apagada()) return;
    try {
        await Haptics.impact({ style: ImpactStyle.Light });
    } catch {
        // Sin motor de vibración (navegador de escritorio): silencio.
    }
}

export async function vibrar_error() {
    if (vibracion_apagada()) return;
    try {
        await Haptics.notification({ type: NotificationType.Error });
    } catch {
        // Sin motor de vibración: silencio.
    }
}