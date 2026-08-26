<template>
	<comp-page :titulo="'Calificaciones'">
		<ion-card>
			<ion-card-header>
				<ion-card-subtitle class="ion-text-uppercase">Evaluaciones</ion-card-subtitle>
				<ion-card-title>Carga de Notas</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<p>Planillas de calificaciones por materia y período lectivo.</p>
				
				<div class="ion-margin-top">
					<ion-badge color="warning">Cierre 1° Trimestre Próximo</ion-badge>
				</div>
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import { IonBadge, IonCard, IonCardContent, IonCardHeader, IonCardSubtitle, IonCardTitle } from '@ionic/vue';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'calificaciones_page',
	components: {
		CompPage: comp_page,
		IonCard,
		IonCardHeader,
		IonCardTitle,
		IonCardSubtitle,
		IonCardContent,
		IonBadge
	}
};
</script>