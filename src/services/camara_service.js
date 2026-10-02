// src/services/camara_service.js
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Capacitor } from '@capacitor/core';

const origenes = {
	camara: CameraSource.Camera,
	galeria: CameraSource.Photos
};

const alias_permiso = {
	camara: 'camera',
	galeria: 'photos'
};

export function camara_disponible(){
	return Capacitor.isNativePlatform();
}

export async function tomar_foto(origen = 'camara'){
	if(!camara_disponible()){
		return {
			ok: false,
			archivo: null,
			mensaje: 'La cámara solo funciona en la app instalada en el dispositivo.'
		};
	}
	try{
		const permiso = await asegurar_permiso(origen);
		if(!permiso.ok) return permiso;
		const foto = await Camera.getPhoto({
			source: origenes[origen] || CameraSource.Camera,
			resultType: CameraResultType.Uri,
			quality: 90,
			width: 1600,
			correctOrientation: true,
			allowEditing: false,
			saveToGallery: false
		});
		const archivo = await foto_a_archivo(foto);
		return {ok: true, archivo, mensaje: null};
	}catch(error){
		if(es_cancelacion(error)) return {ok: false, archivo: null, mensaje: null};
		return {
			ok: false,
			archivo: null,
			mensaje: (error && error.message) || 'No se pudo abrir la cámara.'
		};
	}
}

async function asegurar_permiso(origen){
	const alias = alias_permiso[origen] || 'camera';
	const estado = await Camera.checkPermissions();
	if(permiso_concedido(estado && estado[alias])) return {ok: true, archivo: null, mensaje: null};
	const pedido = await Camera.requestPermissions({permissions: [alias]});
	if(permiso_concedido(pedido && pedido[alias])) return {ok: true, archivo: null, mensaje: null};
	return {
		ok: false,
		archivo: null,
		mensaje: origen == 'galeria'
			? 'Sin permiso para leer las fotos. Habilitalo en los ajustes del teléfono.'
			: 'Sin permiso de cámara. Habilitalo en los ajustes del teléfono.'
	};
}

function permiso_concedido(estado){
	return estado == 'granted' || estado == 'limited';
}

async function foto_a_archivo(foto){
	const ruta = (foto && foto.webPath) || (foto && foto.path);
	if(!ruta) throw new Error('La cámara no devolvió ninguna imagen.');
	const respuesta = await fetch(ruta);
	const blob = await respuesta.blob();
	const extension = foto.format || 'jpeg';
	return new File([blob], `foto_${Date.now()}.${extension}`, {
		type: blob.type || `image/${extension}`,
		lastModified: Date.now()
	});
}

function es_cancelacion(error){
	const mensaje = String((error && error.message) || '');
	return /cancel/i.test(mensaje) || /no image (picked|selected)/i.test(mensaje);
}