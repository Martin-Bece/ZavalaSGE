<template>
	<comp_page titulo="Mis Materias">
		<!-- 1. Buscador Genérico -->
		<comp_buscador
			v-model="store.filtros.busqueda"
			placeholder="Buscar por nombre o código..."
			@buscar="alBuscar"
		/>

		<!-- 2. Estado Carga (Esqueleto) -->
		<div v-if="store.cargando && !store.hay_materias" class="ion-padding">
			<ion-card v-for="i in 3" :key="i" class="card-custom">
				<ion-item lines="none">
					<ion-avatar slot="start">
						<ion-skeleton-text animated />
					</ion-avatar>
					<ion-label>
						<ion-skeleton-text animated style="width: 60%; height: 20px;" />
						<ion-skeleton-text animated style="width: 40%; height: 14px; margin-top: 6px;" />
					</ion-label>
				</ion-item>
			</ion-card>
		</div>

		<!-- 3. Estado Error -->
		<comp_estado_error
			v-else-if="store.error"
			:mensaje="store.error"
			@reintentar="store.cargar()"
		/>

		<!-- 4. Estado Vacío -->
		<comp_estado_vacio
			v-else-if="!store.cargando && !store.hay_materias"
			titulo="Sin materias registradas"
			mensaje="No se encontraron materias que coincidan con la búsqueda."
		/>

		<!-- 5. Lista con Datos -->
		<comp_lista
			v-else
			:cargando="store.cargando"
			:hay_mas="store.hay_mas"
			:mostrados="store.materias.length"
			:total="store.total"
			@cargar_mas="store.cargar_mas()"
		>
			<ion-card v-for="materia in store.materias" :key="materia.id" class="card-custom">
				<ion-item lines="none">
					<ion-avatar slot="start" class="avatar-icono">
						<ion-icon :icon="bookOutline" />
					</ion-avatar>
					<ion-label class="ion-text-wrap">
						<h2><strong>{{ materia.nombre }}</strong></h2>
						<ion-chip color="secondary" class="chip-codigo">
							<ion-icon :icon="barcodeOutline" />
							<ion-label>CÓD: {{ materia.codigo || 'Sin código' }}</ion-label>
						</ion-chip>
					</ion-label>
				</ion-item>

				<ion-card-content class="card-contenido">
					<div class="info-row">
						<ion-icon :icon="schoolOutline" class="info-icon" />
						<span v-if="materia.codigo">Código registrado: <strong>{{ materia.codigo }}</strong></span>
						<ion-badge v-else color="warning" mode="ios">Sin código asignado</ion-badge>
					</div>
				</ion-card-content>
			</ion-card>
		</comp_lista>
	</comp_page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import comp_buscador from '@/components/base/comp_buscador.vue';
import comp_estado_error from '@/components/base/comp_estado_error.vue';
import comp_estado_vacio from '@/components/base/comp_estado_vacio.vue';
import comp_lista from '@/components/base/comp_lista.vue';

import { use_materias_store } from '@/stores/materias_store';
import {
	IonAvatar,
	IonBadge,
	IonCard,
	IonCardContent,
	IonChip,
	IonIcon,
	IonItem,
	IonLabel,
	IonSkeletonText
} from '@ionic/vue';
import {
	barcodeOutline,
	bookOutline,
	schoolOutline
} from 'ionicons/icons';

export default {
	name: 'materias_page',
	components: {
		comp_page,
		comp_buscador,
		comp_estado_error,
		comp_estado_vacio,
		comp_lista,
		IonAvatar,
		IonBadge,
		IonCard,
		IonCardContent,
		IonChip,
		IonIcon,
		IonItem,
		IonLabel,
		IonSkeletonText
	},
	data() {
		return {
			bookOutline,
			barcodeOutline,
			schoolOutline
		};
	},
	computed: {
		store() {
			return use_materias_store();
		}
	},
	methods: {
		alBuscar() {
			this.store.cargar();
		}
	},
	mounted() {
		this.store.cargar();
	}
};
</script>

<style scoped>
.card-custom {
	margin-bottom: 16px;
	border-radius: 14px;
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.avatar-icono {
	background-color: var(--ion-color-secondary);
	color: #fff;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 1.3rem;
}

.chip-codigo {
	margin-left: 0;
	margin-top: 6px;
	height: 24px;
	font-size: 0.8rem;
}

.card-contenido {
	padding-top: 0;
}

.info-row {
	display: flex;
	align-items: center;
	gap: 10px;
	margin-top: 8px;
	color: var(--ion-color-medium-shade);
	font-size: 0.95rem;
}

.info-icon {
	font-size: 1.1rem;
	color: var(--ion-color-secondary);
}
</style>