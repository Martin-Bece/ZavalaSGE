<template>
	<comp-page :titulo="'Académico'">
		<ion-card>
			<ion-card-header>
				<ion-card-subtitle class="ion-text-uppercase">Oferta Educativa</ion-card-subtitle>
				<ion-card-title>Gestión Académica</ion-card-title>
			</ion-card-header>

			<ion-card-content>
				<p>Planes de estudio, cursos y distribución horaria del ciclo lectivo.</p>
				
				<div class="ion-margin-top">
					<ion-badge color="secondary">Plan Vigente 2026</ion-badge>
				</div>
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import { IonBadge, IonCard, IonCardContent, IonCardHeader, IonCardSubtitle, IonCardTitle } from '@ionic/vue';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'academico_page',
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