<template>
	<comp-page 
		:titulo="'Asistencia'" 
		:mostrar_actualizar="true" 
		@actualizar="cargarDatos"
	>
		<!-- Estado de carga: Esqueleto -->
		<div v-if="cargando">
			<ion-card v-for="i in 2" :key="i">
				<ion-card-header>
					<ion-skeleton-text animated style="width: 50%; height: 20px;" />
				</ion-card-header>
				<ion-card-content>
					<ion-skeleton-text animated style="width: 70%; height: 16px;" />
				</ion-card-content>
			</ion-card>
		</div>

		<!-- Estado Sin Datos (Empty State) -->
		<div v-else-if="listaAsistencias.length === 0">
			<ion-card>
				<ion-card-header>
					<ion-card-subtitle class="ion-text-uppercase">Control Diario</ion-card-subtitle>
					<ion-card-title>Toma de Asistencia</ion-card-title>
				</ion-card-header>

				<ion-card-content class="ion-text-center ion-padding-vertical">
					<ion-icon :icon="folderOpenOutline" style="font-size: 48px; color: var(--ion-color-medium);" />
					<p class="ion-margin-top">No hay registros de asistencia cargados aún para el día de hoy.</p>
					
					<div class="ion-margin-top">
						<ion-badge color="warning">Sin registros</ion-badge>
					</div>
				</ion-card-content>
			</ion-card>
		</div>

		<!-- Estado con datos si en un futuro se cargan -->
		<div v-else>
			<ion-card v-for="item in listaAsistencias" :key="item.id">
				<ion-card-header>
					<ion-card-title>{{ item.alumno }}</ion-card-title>
				</ion-card-header>
				<ion-card-content>
					<p>Estado: {{ item.estado }}</p>
				</ion-card-content>
			</ion-card>
		</div>
	</comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import {
	IonBadge,
	IonCard,
	IonCardContent,
	IonCardHeader,
	IonCardSubtitle,
	IonCardTitle,
	IonIcon,
	IonSkeletonText
} from '@ionic/vue';
import { folderOpenOutline } from 'ionicons/icons';

// Función que simula la promesa retornando un arreglo vacío
function obtener_asistencias() {
	return new Promise((resolve) => {
		setTimeout(() => {
			resolve([]);
		}, 600);
	});
}

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
		IonIcon,
		IonSkeletonText
	},
	data() {
		return {
			listaAsistencias: [],
			cargando: true
		};
	},
	setup() {
		return { folderOpenOutline };
	},
	methods: {
		async cargarDatos() {
			this.cargando = true;
			try {
				this.listaAsistencias = await obtener_asistencias();
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