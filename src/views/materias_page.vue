<template>
	<comp-page 
		:titulo="'Mis Materias'" 
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
			<ion-card v-for="materia in listaMaterias" :key="materia.id">
				<ion-card-header>
					<ion-card-subtitle>{{ materia.curso }}</ion-card-subtitle>
					<ion-card-title>{{ materia.nombre }}</ion-card-title>
				</ion-card-header>

				<ion-card-content>
					<p>
						<strong>Código:</strong> 
						<span v-if="materia.codigo">{{ materia.codigo }}</span>
						<ion-badge v-else color="warning">Sin código asignado</ion-badge>
					</p>
				</ion-card-content>
			</ion-card>
		</div>
	</comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_materias } from '@/datos/materias';
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
	name: 'materias_page',
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
			listaMaterias: [],
			cargando: true
		};
	},
	methods: {
		async cargarDatos() {
			this.cargando = true;
			try {
				this.listaMaterias = await obtener_materias();
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