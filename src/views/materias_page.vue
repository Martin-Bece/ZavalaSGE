<template>
	<comp_page
		:titulo="elegida ? elegida.materiaNombre : 'Mis Materias'"
		:mostrar_actualizar="true"
		@actualizar="recargar"
	>
		<div v-if="elegida" class="barra-volver">
			<ion-button fill="clear" size="small" @click="volver">
				<ion-icon slot="start" :icon="arrowBackOutline" />
				Volver
			</ion-button>
			<span class="subtitulo">{{ elegida.cursoNombre }} · {{ elegida.comisionNombre }}</span>
		</div>

		<div v-if="store.cargando" class="ion-padding">
			<ion-card v-for="i in 3" :key="i" class="card-custom">
				<ion-item lines="none">
					<ion-avatar slot="start">
						<ion-skeleton-text animated />
					</ion-avatar>
					<ion-label>
						<ion-skeleton-text animated style="width: 60%; height: 20px" />
						<ion-skeleton-text animated style="width: 40%; height: 14px; margin-top: 6px" />
					</ion-label>
				</ion-item>
			</ion-card>
		</div>

		<comp_estado_error
			v-else-if="store.error"
			:mensaje="store.error"
			@reintentar="recargar"
		/>

		<!-- NIVEL 2: alumnos de la materia/comisión elegida -->
		<comp_alumnos_comision
			v-else-if="elegida"
			:alumnos="store.alumnos"
			@abrir="abrir_alumno"
		/>

		<!-- NIVEL 1: materias asignadas -->
		<template v-else>
			<comp_estado_vacio
				v-if="!store.hay_asignaciones"
				titulo="Sin materias asignadas"
				mensaje="Todavía no tenés materias asignadas."
			/>

			<ion-card
				v-for="asignacion in store.asignaciones"
				:key="asignacion.id"
				button
				class="card-custom"
				@click="elegir(asignacion)"
			>
				<ion-item lines="none">
					<ion-avatar slot="start" class="avatar-icono">
						<ion-icon :icon="bookOutline" />
					</ion-avatar>
					<ion-label class="ion-text-wrap">
						<h2><strong>{{ asignacion.materiaNombre }}</strong></h2>
						<div class="chips-row">
							<ion-chip color="tertiary" class="chip-dato">
								<ion-icon :icon="layersOutline" />
								<ion-label>{{ asignacion.cursoNombre }} · {{ asignacion.comisionNombre }}</ion-label>
							</ion-chip>
							<ion-chip color="primary" class="chip-dato">
								<ion-icon :icon="peopleOutline" />
								<ion-label>{{ asignacion.cantidadAlumnos }} alumnos</ion-label>
							</ion-chip>
						</div>
					</ion-label>
				</ion-item>
			</ion-card>
		</template>
	</comp_page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import comp_estado_error from '@/components/base/comp_estado_error.vue';
import comp_estado_vacio from '@/components/base/comp_estado_vacio.vue';
import comp_alumnos_comision from '@/components/dominio/comp_alumnos_comision.vue';

import { use_mis_alumnos_store } from '@/stores/mis_alumnos_store';
import {
	IonAvatar,
	IonButton,
	IonCard,
	IonChip,
	IonIcon,
	IonItem,
	IonLabel,
	IonSkeletonText
} from '@ionic/vue';
import {
	arrowBackOutline,
	bookOutline,
	layersOutline,
	peopleOutline
} from 'ionicons/icons';

export default {
	name: 'materias_page',
	components: {
		comp_page,
		comp_estado_error,
		comp_estado_vacio,
		comp_alumnos_comision,
		IonAvatar,
		IonButton,
		IonCard,
		IonChip,
		IonIcon,
		IonItem,
		IonLabel,
		IonSkeletonText
	},
	data() {
		return {
			elegida: null,
			arrowBackOutline,
			bookOutline,
			layersOutline,
			peopleOutline
		};
	},
	computed: {
		store() {
			return use_mis_alumnos_store();
		}
	},
	methods: {
		async elegir(asignacion) {
			this.elegida = asignacion;
			await this.store.abrir_comision(asignacion.comisionId);
		},
		volver() {
			this.elegida = null;
			this.store.comision = null;
			this.store.alumnos = [];
			this.store.error = null;
		},
		async recargar() {
			if (this.elegida) await this.elegir(this.elegida);
			else await this.store.cargar_asignaciones();
		},
		abrir_alumno(alumno) {
			this.$router.push(`/app/alumno/${alumno.id}`);
		}
	},
	// Recarga lo que se está mirando en vez de volver siempre a la lista.
	ionViewDidEnter() {
		this.recargar();
	}
};
</script>

<style scoped>
.card-custom {
	margin: 12px 16px;
	border-radius: 14px;
	background: var(--ion-card-background, #ffffff);
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
	border: 1px solid rgba(255, 255, 255, 0.05);
}

body.dark .card-custom {
	background: #1e293b;
	border: 1px solid rgba(255, 255, 255, 0.1);
}

.avatar-icono {
	background-color: var(--ion-color-secondary);
	color: #fff;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 1.3rem;
}

.chips-row {
	display: flex;
	align-items: center;
	gap: 8px;
	flex-wrap: wrap;
	margin-top: 6px;
}

.chip-dato {
	margin: 0;
	height: 24px;
	font-size: 0.8rem;
}

.barra-volver {
	display: flex;
	align-items: center;
	gap: 8px;
	padding: 8px 16px 0;
}

.subtitulo {
	opacity: 0.8;
	font-size: 0.95rem;
}
</style>