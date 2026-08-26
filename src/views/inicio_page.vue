<template>
	<comp-page 
		:titulo="'Inicio'"
		:mostrar_actualizar="true">
		<!-- Tarjeta de Bienvenida Principal -->
		<ion-card>
			<ion-card-header>
				<ion-card-subtitle class="ion-text-uppercase">Escuela N° 4</ion-card-subtitle>
				<ion-card-title>Bienvenido a ZavalaSGE</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<p>Sistema de Gestión Escolar cargado y listo. Seleccioná una de las acciones rápidas para comenzar.</p>

				<div class="ion-margin-top">
					<ion-badge color="secondary">
						<ion-icon :icon="checkmarkCircleOutline" /> Sistema Operativo
					</ion-badge>
				</div>
			</ion-card-content>
		</ion-card>

		<!-- Acciones Rápidas con Uso Semántico de la Paleta -->
		<ion-card>
			<ion-card-header>
				<ion-card-title>Acceso Rápido</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<div class="ion-flex ion-flex-wrap gap-2">
					<!-- Azul: Acción Principal de Navegación -->
					<ion-button color="primary" fill="solid" router-link="/materias">
						<ion-icon slot="start" :icon="bookOutline" />
						Mis Materias
					</ion-button>

					<!-- Rojo: Control Crítico / Diario -->
					<ion-button color="danger" fill="outline" router-link="/asistencia">
						<ion-icon slot="start" :icon="checkboxOutline" />
						Tomar Asistencia
					</ion-button>

					<!-- Naranja: Cierre / Alertas -->
					<ion-button color="warning" fill="outline" router-link="/calificaciones">
						<ion-icon slot="start" :icon="createOutline" />
						Cargar Notas
					</ion-button>
				</div>
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import {
	IonBadge,
	IonButton,
	IonCard,
	IonCardContent,
	IonCardHeader,
	IonCardSubtitle,
	IonCardTitle,
	IonIcon
} from '@ionic/vue';
import {
	bookOutline,
	checkboxOutline,
	checkmarkCircleOutline,
	createOutline
} from 'ionicons/icons';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'inicio_page',
	components: {
		CompPage: comp_page,
		IonBadge,
		IonButton,
		IonCard,
		IonCardContent,
		IonCardHeader,
		IonCardSubtitle,
		IonCardTitle,
		IonIcon
	},
	setup() {
		return {
			bookOutline,
			checkboxOutline,
			checkmarkCircleOutline,
			createOutline
		};
	}
};
</script>