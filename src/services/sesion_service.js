import { enviar, obtener } from '@/services/ajax_service';

export function iniciar_sesion(email, password) {
	return enviar('api/sesion/login', 'POST', { email, password });
}

export function cerrar_sesion(refresh_token, todos = false) {
	return enviar('api/sesion/logout', 'POST', { refresh_token, todos });
}

export function obtener_mi_usuario() {
	return obtener('api/sesion/yo');
}