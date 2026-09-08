<template>
	<comp_page titulo="Gestión de Personal">
		<!-- 1. Buscador Genérico -->
		<comp_buscador
			v-model="store.filtros.busqueda"
			placeholder="Buscar por DNI, nombre o teléfono..."
			@buscar="alBuscar"
		/>

		<!-- Botón para Alta -->
		<div class="ion-padding-horizontal ion-margin-top">
			<ion-button expand="block" @click="abrirModalCrear">
				<ion-icon slot="start" :icon="addOutline" />
				Nueva Persona
			</ion-button>
		</div>

		<!-- 2. Estado Carga (Esqueleto) -->
		<div v-if="store.cargando && !store.hay_personas" class="ion-padding">
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
			v-else-if="!store.cargando && !store.hay_personas"
			titulo="Sin registros"
			mensaje="No se encontraron personas cargadas o que coincidan con la búsqueda."
		/>

		<!-- 5. Lista con Datos -->
		<comp_lista
			v-else
			:cargando="store.cargando"
			:hay_mas="store.hay_mas"
			:mostrados="store.personas.length"
			:total="store.total"
			@cargar_mas="store.cargar_mas()"
		>
			<ion-card v-for="persona in store.personas" :key="persona.id" class="card-custom">
				<ion-item lines="none">
					<ion-avatar slot="start" class="avatar-iniciales">
						<span>{{ obtenerIniciales(persona.nombre, persona.apellido) }}</span>
					</ion-avatar>
					<ion-label class="ion-text-wrap">
						<h2><strong>{{ persona.apellido }}, {{ persona.nombre }}</strong></h2>
						<ion-chip color="primary" class="chip-dni">
							<ion-icon :icon="cardOutline" />
							<ion-label>DNI: {{ persona.dni }}</ion-label>
						</ion-chip>
					</ion-label>
				</ion-item>

				<ion-card-content class="card-contenido">
					<div class="info-row">
						<ion-icon :icon="callOutline" class="info-icon" />
						<span>{{ persona.telefono || 'Sin teléfono' }}</span>
					</div>

					<div class="info-row">
						<ion-icon :icon="locationOutline" class="info-icon" />
						<span v-if="persona.direccion">{{ persona.direccion }}</span>
						<ion-badge v-else color="warning" mode="ios">Sin dirección registrada</ion-badge>
					</div>

					<!-- Botones de Acción (Modificación / Baja) -->
					<div class="acciones-row ion-margin-top">
						<ion-button fill="clear" size="small" color="primary" @click="abrirModalEditar(persona)">
							<ion-icon slot="start" :icon="createOutline" />
							Editar
						</ion-button>
						<ion-button fill="clear" size="small" color="danger" @click="confirmarEliminar(persona)">
							<ion-icon slot="start" :icon="trashOutline" />
							Eliminar
						</ion-button>
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

import { use_personas_store } from '@/stores/personas_store';
import { 
	IonAvatar, 
	IonBadge, 
	IonButton, 
	IonCard, 
	IonCardContent, 
	IonChip, 
	IonIcon, 
	IonItem, 
	IonLabel, 
	IonSkeletonText 
} from '@ionic/vue';
import { 
	addOutline, 
	cardOutline, 
	callOutline, 
	createOutline, 
	locationOutline, 
	trashOutline 
} from 'ionicons/icons';

export default {
	name: 'personal_page',
	components: {
		comp_page,
		comp_buscador,
		comp_estado_error,
		comp_estado_vacio,
		comp_lista,
		IonAvatar,
		IonBadge,
		IonButton,
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
			addOutline,
			cardOutline,
			callOutline,
			createOutline,
			locationOutline,
			trashOutline
		};
	},
	computed: {
		store() {
			return use_personas_store();
		}
	},
	methods: {
		alBuscar() {
			this.store.cargar();
		},
		obtenerIniciales(nombre, apellido) {
			const n = nombre ? nombre.charAt(0) : '';
			const a = apellido ? apellido.charAt(0) : '';
			return `${n}${a}`.toUpperCase();
		},
		abrirModalCrear() {
			// Próximo paso: abrir modal de formulario para crear
		},
		abrirModalEditar(persona) {
			// Próximo paso: abrir modal con los datos de 'persona' para editar
		},
		async confirmarEliminar(persona) {
			if (confirm(`¿Seguro que deseas eliminar a ${persona.nombre} ${persona.apellido}?`)) {
				await this.store.eliminar(persona.id);
			}
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

.avatar-iniciales {
	background-color: var(--ion-color-primary);
	color: #fff;
	display: flex;
	align-items: center;
	justify-content: center;
	font-weight: bold;
	font-size: 1.1rem;
}

.chip-dni {
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
	color: var(--ion-color-primary);
}

.acciones-row {
	display: flex;
	justify-content: flex-end;
	border-top: 1px solid var(--ion-color-light);
	padding-top: 8px;
}
</style>