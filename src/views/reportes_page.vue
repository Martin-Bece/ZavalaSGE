<template>
	<comp-page :titulo="'Reportes'">
		<ion-card>
			<ion-card-header>
				<ion-card-title>Reportes e Informes</ion-card-title>
			</ion-card-header>
			<ion-card-content>
				Generación de estadísticas e informes institucionales.
			</ion-card-content>
		</ion-card>
	</comp-page>
</template>

<script>
import { IonCard, IonCardContent, IonCardHeader, IonCardTitle } from '@ionic/vue';
import comp_page from '@/components/estructura/comp_page.vue';

export default {
	name: 'reportes_page',
	components: {
		CompPage: comp_page,
		IonCard,
		IonCardContent,
		IonCardHeader,
		IonCardTitle
	}
};
</script>