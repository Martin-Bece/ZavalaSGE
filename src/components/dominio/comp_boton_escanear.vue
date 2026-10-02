<template>
	<ion-button :fill="fill" :size="size" :expand="expand" :disabled="escaneando" @click="escanear">
		<ion-spinner v-if="escaneando" name="crescent" />
		<template v-else>
			<ion-icon slot="start" :icon="qrCodeOutline" />
			Escanear QR
		</template>
	</ion-button>
</template>

<script>
import { escanear_qr, escaner_disponible, id_de_alumno_desde_codigo } from '@/services/qr_service';
import { alertController, IonButton, IonIcon, IonSpinner, toastController } from '@ionic/vue';
import { qrCodeOutline } from 'ionicons/icons';

// Lee el QR de la constancia y abre la ficha de ese alumno. Quien no tenga acceso a ese alumno
// ve el aviso de la API (403): el QR solo trae el código, no abre nada por sí mismo.
export default {
	name: 'comp_boton_escanear',
	components: {
		IonButton,
		IonIcon,
		IonSpinner
	},
	props: {
		expand: {
			type: String,
			default: undefined
		},
		fill: {
			type: String,
			default: 'outline'
		},
		size: {
			type: String,
			default: 'small'
		}
	},
	data() {
		return {
			escaneando: false,
			qrCodeOutline
		};
	},
	methods: {
		async escanear() {
			var vm = this;
			if (!escaner_disponible()) return vm.pedir_codigo();

			vm.escaneando = true;
			try {
				const resultado = await escanear_qr();
				if (!resultado.ok) {
					if (resultado.mensaje) await vm.avisar(resultado.mensaje);
					return;
				}
				await vm.abrir_por_codigo(resultado.contenido);
			} finally {
				vm.escaneando = false;
			}
		},
		async abrir_por_codigo(texto) {
			var vm = this;
			const id = id_de_alumno_desde_codigo(texto);
			if (!id) {
				await vm.avisar(`«${texto}» no es el código de una constancia.`);
				return;
			}
			vm.$router.push(`/app/alumno/${id}`);
		},
		// Sin escáner (navegador): un alert para tipear el código impreso debajo del QR.
		async pedir_codigo() {
			var vm = this;
			const alerta = await alertController.create({
				header: 'Código de la constancia',
				message: 'Escribí el código que figura debajo del QR (por ejemplo, ALU-0001).',
				inputs: [{ name: 'codigo', type: 'text', placeholder: 'ALU-0001' }],
				buttons: [
					{ text: 'Cancelar', role: 'cancel' },
					{
						text: 'Abrir',
						handler: (valores) => {
							vm.abrir_por_codigo(valores.codigo);
						}
					}
				]
			});
			await alerta.present();
		},
		async avisar(mensaje) {
			const toast = await toastController.create({ message: mensaje, duration: 2500, position: 'bottom' });
			await toast.present();
		}
	}
};
</script>