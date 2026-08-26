<template>
	<comp-page :titulo="'Mi Perfil'">
		<ion-card>
			<ion-card-header>
				<ion-card-subtitle class="ion-text-uppercase">Configuración</ion-card-subtitle>
				<ion-card-title>Perfil de Usuario</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<p>Información personal y preferencias de la cuenta.</p>
				
				<div class="ion-margin-top">
					<ion-button color="primary" size="small">
						Editar Datos
					</ion-button>
				</div>
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import { IonButton, IonCard, IonCardContent, IonCardHeader, IonCardSubtitle, IonCardTitle } from '@ionic/vue';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'perfil_page',
	components: {
		CompPage: comp_page,
		IonCard,
		IonCardHeader,
		IonCardTitle,
		IonCardSubtitle,
		IonCardContent,
		IonButton
	}
};
</script>