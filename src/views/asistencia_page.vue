<template>
	<comp-page :titulo="'Asistencia'">
		<ion-card>
			<ion-card-header>
				<ion-card-subtitle class="ion-text-uppercase">Control Diario</ion-card-subtitle>
				<ion-card-title>Toma de Asistencia</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<p>Registro diario de inasistencias y faltas justificadas.</p>
				
				<div class="ion-margin-top">
					<ion-badge color="danger">
						<ion-icon :icon="alertCircleOutline" /> 3 Alumnos Ausentes
					</ion-badge>
				</div>
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import { IonBadge, IonCard, IonCardContent, IonCardHeader, IonCardSubtitle, IonCardTitle, IonIcon } from '@ionic/vue';
import { alertCircleOutline } from 'ionicons/icons';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'asistencia_page',
	components: {
		CompPage: comp_page,
		IonCard,
		IonCardHeader,
		IonCardTitle,
		IonCardSubtitle,
		IonCardContent,
		IonBadge,
		IonIcon
	},
	setup() {
		return { alertCircleOutline };
	}
};
</script>