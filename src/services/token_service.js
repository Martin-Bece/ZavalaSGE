import { SecureStorage } from '@aparajita/capacitor-secure-storage';

const CLAVE_TOKEN = 'gestor_token';
const CLAVE_REFRESH = 'gestor_refresh';
const CLAVE_EXPIRA = 'gestor_expira';

let token_en_memoria = null;
let al_expirar_sesion = null;

async function guardar(clave, valor) {
	if (valor === null || valor === undefined) {
		await SecureStorage.remove(clave);
		return;
	}
	await SecureStorage.set(clave, String(valor));
}

async function leer(clave) {
	try {
		return await SecureStorage.get(clave);
	} catch {
		return null;
	}
}

export async function guardar_sesion(token, expira_en, refresh_token, refresh_expira_en) {
	token_en_memoria = token;
	await guardar(CLAVE_TOKEN, token);
	await guardar(CLAVE_EXPIRA, expira_en);
	await guardar(CLAVE_REFRESH, refresh_token);
	return refresh_expira_en;
}

export async function restaurar_sesion() {
	token_en_memoria = await leer(CLAVE_TOKEN);
	return token_en_memoria;
}

export async function borrar_sesion() {
	token_en_memoria = null;
	await SecureStorage.remove(CLAVE_TOKEN);
	await SecureStorage.remove(CLAVE_REFRESH);
	await SecureStorage.remove(CLAVE_EXPIRA);
}

export function obtener_refresh_token() {
	return leer(CLAVE_REFRESH);
}

export function cabecera_autorizacion() {
	return token_en_memoria ? { Authorization: `Bearer ${token_en_memoria}` } : {};
}

export function al_expirar(callback) {
	al_expirar_sesion = callback;
}

export function notificar_sesion_expirada() {
	token_en_memoria = null;
	if (al_expirar_sesion) al_expirar_sesion();
}