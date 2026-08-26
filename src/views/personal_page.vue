<template>
	<comp-page 
		:titulo="'Gestión de Personal'" 
		:mostrar_actualizar="true" 
		@actualizar="cargarDatos"
	>
		<!-- Estado de carga: Esqueleto -->
		<div v-if="cargando">
			<ion-card v-for="i in 3" :key="i">
				<ion-card-header>
					<ion-skeleton-text animated style="width: 60%; height: 20px;" />
				</ion-card-header>
				<ion-card-content>
					<ion-skeleton-text animated style="width: 40%; height: 16px;" />
					<ion-skeleton-text animated style="width: 80%; height: 16px; margin-top: 8px;" />
				</ion-card-content>
			</ion-card>
		</div>

		<!-- Estado con datos cargados -->
		<div v-else>
			<ion-card v-for="persona in listaPersonas" :key="persona.id">
				<ion-card-header>
					<ion-card-subtitle>{{ persona.rol }}</ion-card-subtitle>
					<ion-card-title>{{ persona.nombre }} {{ persona.apellido }}</ion-card-title>
				</ion-card-header>

				<ion-card-content>
					<p><strong>DNI:</strong> {{ persona.dni || 'Sin registrar' }}</p>
					<p><strong>Teléfono:</strong> {{ persona.telefono }}</p>
					<p>
						<strong>Dirección:</strong> 
						<span v-if="persona.direccion">{{ persona.direccion }}</span>
						<ion-badge v-else color="warning">Sin dirección registrada</ion-badge>
					</p>
				</ion-card-content>
			</ion-card>
		</div>
	</comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_personas } from '@/datos/personas';
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
	name: 'personal_page',
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
			listaPersonas: [],
			cargando: true
		};
	},
	methods: {
		async cargarDatos() {
			this.cargando = true;
			try {
				this.listaPersonas = await obtener_personas();
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