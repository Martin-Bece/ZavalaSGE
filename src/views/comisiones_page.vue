<template>
	<comp-page 
		:titulo="'Comisiones'" 
		:mostrar_actualizar="true" 
		@actualizar="cargarDatos"
	>
		<!-- Estado de carga: Esqueleto -->
		<div v-if="cargando">
			<ion-card v-for="i in 3" :key="i">
				<ion-card-header>
					<ion-skeleton-text animated style="width: 50%; height: 20px;" />
				</ion-card-header>
				<ion-card-content>
					<ion-skeleton-text animated style="width: 70%; height: 16px;" />
				</ion-card-content>
			</ion-card>
		</div>

		<!-- Estado con datos cargados -->
		<div v-else>
			<ion-card v-for="comision in listaComisiones" :key="comision.id">
				<ion-card-header>
					<ion-card-subtitle>{{ comision.curso }}</ion-card-subtitle>
					<ion-card-title>{{ comision.nombre }}</ion-card-title>
				</ion-card-header>

				<ion-card-content>
					<p>
						<strong>Preceptor a Cargo:</strong> 
						<span v-if="comision.preceptor">{{ comision.preceptor }}</span>
						<ion-badge v-else color="warning">Sin preceptor asignado</ion-badge>
					</p>
				</ion-card-content>
			</ion-card>
		</div>
	</comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_comisiones } from '@/datos/comisiones';
import {
	IonBadge,
	IonCard,
	IonCardContent,
	IonCardHeader,
	IonCardSubtitle,
	IonCardTitle,
	IonSkeletonText
} from '@ionic/vue';

export default {
	name: 'comisiones_page',
	components: {
		CompPage: comp_page,
		IonCard,
		IonCardHeader,
		IonCardTitle,
		IonCardSubtitle,
		IonCardContent,
		IonSkeletonText,
		IonBadge
	},
	data() {
		return {
			listaComisiones: [],
			cargando: true
		};
	},
	methods: {
		async cargarDatos() {
			this.cargando = true;
			try {
				this.listaComisiones = await obtener_comisiones();
			} finally {
				this.cargando = false;
			}
		}
	},
	mounted() {
		this.cargarDatos();
	}
};
</script>