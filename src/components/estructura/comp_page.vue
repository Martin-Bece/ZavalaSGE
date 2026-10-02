<template>
	<ion-page>
		<ion-header>
			<ion-toolbar color="primary">
				<ion-buttons slot="start">
					<ion-menu-button />
				</ion-buttons>
				<ion-title>{{ titulo }}</ion-title>
				<ion-buttons v-if="mostrar_campana" slot="end">
					<ion-button class="boton-campana" @click="abrir_notificaciones">
						<ion-icon slot="icon-only" :icon="notificationsOutline" />
						<ion-badge v-if="notificaciones_store.no_leidas > 0" color="danger" class="globo">
							{{ notificaciones_store.no_leidas > 99 ? '99+' : notificaciones_store.no_leidas }}
						</ion-badge>
					</ion-button>
				</ion-buttons>
			</ion-toolbar>
		</ion-header>

		<ion-content :fullscreen="true" class="ion-padding">
			<ion-refresher v-if="mostrar_actualizar" slot="fixed" @ionRefresh="refrescar">
				<ion-refresher-content 
					pulling-text="Desliza para actualizar" 
					refreshing-text="Actualizando..." 
				/>
			</ion-refresher>

			<slot />
		</ion-content>
	</ion-page>
</template>

<script>
import { use_notificaciones_store } from '@/stores/notificaciones_store';
import {
	IonBadge,
	IonButton,
	IonButtons,
	IonContent,
	IonHeader,
	IonIcon,
	IonMenuButton,
	IonPage,
	IonRefresher,
	IonRefresherContent,
	IonTitle,
	IonToolbar
} from '@ionic/vue';
import { notificationsOutline } from 'ionicons/icons';

export default {
	name: 'comp_page',
	components: {
		IonBadge,
		IonButton,
		IonButtons,
		IonContent,
		IonHeader,
		IonIcon,
		IonMenuButton,
		IonPage,
		IonRefresher,
		IonRefresherContent,
		IonTitle,
		IonToolbar
	},
	emits: [
		'actualizar'
	],
	props: {
		mostrar_actualizar: {
			type: Boolean,
			default: false
		},
		// La campana de la bandeja. Se oculta en la propia pantalla de notificaciones.
		mostrar_campana: {
			type: Boolean,
			default: true
		},
		titulo: {
			type: String,
			required: true
		}
	},
	data() {
		return {
			notificationsOutline
		};
	},
	computed: {
		notificaciones_store() {
			return use_notificaciones_store();
		}
	},
	methods: {
		abrir_notificaciones: function () {
			var vm = this;
			vm.$router.push('/app/notificaciones');
		},
		refrescar: function (evento) {
			var vm = this;
			vm.$emit('actualizar');
			// El spinner se cierra a mano: sin datos que esperar, Ionic lo dejaria girando.
			window.setTimeout(() => evento.detail.complete(), 350);
		}
	}
};
</script>

<style scoped>
.globo {
	position: absolute;
	top: 4px;
	right: 2px;
	min-width: 16px;
	height: 16px;
	padding: 0 4px;
	font-size: 0.65rem;
	line-height: 16px;
	border-radius: 8px;
	pointer-events: none;
}
</style>