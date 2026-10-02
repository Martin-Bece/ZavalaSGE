<template>
	<comp_page :titulo="titulo" :mostrar_actualizar="true" @actualizar="cargar">
		<div class="barra-volver">
			<ion-button fill="clear" size="small" @click="$router.back()">
				<ion-icon slot="start" :icon="arrowBackOutline" />
				Volver
			</ion-button>
		</div>

		<div v-if="store.cargando && !alumno" class="ion-padding">
			<ion-card class="card-custom">
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
			v-else-if="store.error && !alumno"
			:mensaje="store.error"
			@reintentar="cargar"
		/>

		<template v-else-if="alumno">
			<!-- Cabecera -->
			<ion-card class="card-custom">
				<ion-item lines="none">
					<ion-avatar slot="start" class="avatar-iniciales">
						<span>{{ iniciales }}</span>
					</ion-avatar>
					<ion-label class="ion-text-wrap">
						<h2><strong>{{ alumno.apellido }}, {{ alumno.nombre }}</strong></h2>
						<div class="chips-row">
							<ion-chip color="primary" class="chip-dato">
								<ion-icon :icon="cardOutline" />
								<ion-label>DNI: {{ alumno.dni }}</ion-label>
							</ion-chip>
							<ion-badge :color="color_estado" mode="ios" class="badge-estado">
								{{ alumno.estado }}
							</ion-badge>
						</div>
					</ion-label>
				</ion-item>
			</ion-card>

			<!-- Datos de la persona: solo lectura -->
			<ion-card class="card-custom">
				<ion-card-header>
					<ion-card-subtitle class="ion-text-uppercase">Datos personales</ion-card-subtitle>
				</ion-card-header>
				<ion-card-content>
					<div class="info-row">
						<ion-icon :icon="callOutline" class="info-icon" />
						<span>{{ alumno.telefono || 'Sin teléfono' }}</span>
					</div>
					<div class="info-row">
						<ion-icon :icon="locationOutline" class="info-icon" />
						<span>{{ alumno.direccion || 'Sin dirección' }}</span>
					</div>
					<div class="info-row">
						<ion-icon :icon="calendarOutline" class="info-icon" />
						<span>{{ fecha_nacimiento }}</span>
					</div>
				</ion-card-content>
			</ion-card>

			<!-- Datos del alumno: editables -->
			<ion-card class="card-custom">
				<ion-card-header>
					<ion-card-subtitle class="ion-text-uppercase">Datos del alumno</ion-card-subtitle>
				</ion-card-header>
				<ion-card-content>
					<ion-item>
						<ion-input
							v-model="form.legajo"
							label="Legajo"
							label-placement="stacked"
							:readonly="!editando"
						/>
					</ion-item>
					<ion-item lines="none">
						<ion-select
							v-model="form.estado"
							label="Estado"
							label-placement="stacked"
							interface="popover"
							:disabled="!editando"
						>
							<ion-select-option value="regular">Regular</ion-select-option>
							<ion-select-option value="libre">Libre</ion-select-option>
							<ion-select-option value="baja">Baja</ion-select-option>
						</ion-select>
					</ion-item>

					<div v-if="puede_gestionar" class="acciones-row ion-margin-top">
						<template v-if="!editando">
							<ion-button fill="clear" size="small" @click="editando = true">
								<ion-icon slot="start" :icon="createOutline" />
								Editar
							</ion-button>
						</template>
						<template v-else>
							<ion-button fill="clear" size="small" color="medium" @click="cancelar">
								<ion-icon slot="start" :icon="closeOutline" />
								Cancelar
							</ion-button>
							<ion-button size="small" :disabled="store.guardando" @click="guardar">
								<ion-icon slot="start" :icon="saveOutline" />
								Guardar
							</ion-button>
						</template>
					</div>
				</ion-card-content>
			</ion-card>

			<!-- Constancia -->
			<div v-if="puede_gestionar" class="ion-padding-horizontal">
				<ion-button
					v-if="alumno.estado === 'regular'"
					expand="block"
					:disabled="compartiendo || editando"
					@click="compartir_constancia"
				>
					<ion-spinner v-if="compartiendo" name="crescent" />
					<template v-else>
						<ion-icon slot="start" :icon="documentTextOutline" />
						Constancia de alumno regular
					</template>
				</ion-button>
				<ion-note v-else class="nota-constancia">
					La constancia solo se emite a alumnos regulares.
				</ion-note>
			</div>
		</template>
	</comp_page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import comp_estado_error from '@/components/base/comp_estado_error.vue';

import { descargar_constancia_alumno } from '@/services/mis_alumnos_service';
import { compartir_archivo } from '@/services/compartir_service';
import { use_mis_alumnos_store } from '@/stores/mis_alumnos_store';
import { use_sesion_store } from '@/stores/sesion_store';
import {
	IonAvatar,
	IonBadge,
	IonButton,
	IonCard,
	IonCardContent,
	IonCardHeader,
	IonCardSubtitle,
	IonChip,
	IonIcon,
	IonInput,
	IonItem,
	IonLabel,
	IonNote,
	IonSelect,
	IonSelectOption,
	IonSkeletonText,
	IonSpinner,
	toastController
} from '@ionic/vue';
import {
	arrowBackOutline,
	calendarOutline,
	callOutline,
	cardOutline,
	closeOutline,
	createOutline,
	documentTextOutline,
	locationOutline,
	saveOutline
} from 'ionicons/icons';

export default {
	name: 'alumno_detalle_page',
	components: {
		comp_page,
		comp_estado_error,
		IonAvatar,
		IonBadge,
		IonButton,
		IonCard,
		IonCardContent,
		IonCardHeader,
		IonCardSubtitle,
		IonChip,
		IonIcon,
		IonInput,
		IonItem,
		IonLabel,
		IonNote,
		IonSelect,
		IonSelectOption,
		IonSkeletonText,
		IonSpinner
	},
	data() {
		return {
			editando: false,
			compartiendo: false,
			form: { legajo: '', estado: 'regular' },
			arrowBackOutline,
			calendarOutline,
			callOutline,
			cardOutline,
			closeOutline,
			createOutline,
			documentTextOutline,
			locationOutline,
			saveOutline
		};
	},
	computed: {
		store() {
			return use_mis_alumnos_store();
		},
		alumno() {
			return this.store.alumno;
		},
		titulo() {
			return this.alumno ? `${this.alumno.apellido}, ${this.alumno.nombre}` : 'Alumno';
		},
		iniciales() {
			if (!this.alumno) return '??';
			return `${this.alumno.apellido.charAt(0)}${this.alumno.nombre.charAt(0)}`.toUpperCase();
		},
		color_estado() {
			if (!this.alumno) return 'medium';
			return { regular: 'success', libre: 'warning', baja: 'danger' }[this.alumno.estado] || 'medium';
		},
		fecha_nacimiento() {
			const f = this.alumno && this.alumno.fechaNacimiento;
			if (!f) return 'Sin fecha de nacimiento';
			const [anio, mes, dia] = String(f).split('-');
			return `${dia}/${mes}/${anio}`;
		},
		// Admin y preceptor editan y emiten la constancia; el docente solo ve.
		puede_gestionar() {
			const rol = use_sesion_store().rol_activo;
			return rol === 'ADMIN' || rol === 'PRECEPTOR';
		}
	},
	methods: {
		async cargar() {
			const id = this.$route.params.id;
			if (!id) return;
			this.editando = false;
			const alumno = await this.store.cargar_alumno(id);
			if (alumno) this.copiar_al_formulario();
		},
		copiar_al_formulario() {
			this.form = { legajo: this.alumno.legajo, estado: this.alumno.estado };
		},
		cancelar() {
			this.copiar_al_formulario();
			this.editando = false;
		},
		async guardar() {
			const legajo = this.form.legajo.trim();
			if (!legajo) return this.avisar('El legajo es obligatorio.');
			const ok = await this.store.guardar_datos_alumno(this.alumno.id, {
				legajo,
				estado: this.form.estado
			});
			if (ok) {
				this.copiar_al_formulario();
				this.editando = false;
				await this.avisar('Datos guardados.');
			} else {
				await this.avisar(this.store.error || 'No se pudieron guardar los datos.');
				this.store.error = null;
			}
		},
		async compartir_constancia() {
			var vm = this;
			const alumno = vm.alumno;
			const codigo = `ALU-${String(alumno.id).padStart(4, '0')}`;
			vm.compartiendo = true;
			try {
				const blob = await descargar_constancia_alumno(alumno.id);
				const resultado = await compartir_archivo({
					nombre_archivo: `constancia-${codigo}.pdf`,
					blob,
					titulo: `Constancia ${codigo}`,
					texto: `Constancia de alumno regular de ${alumno.apellido} ${alumno.nombre}`
				});
				if (resultado.mensaje) await vm.avisar(resultado.mensaje);
			} catch (error) {
				await vm.avisar(error.mensaje || 'No se pudo generar la constancia.');
			} finally {
				vm.compartiendo = false;
			}
		},
		async avisar(mensaje) {
			const toast = await toastController.create({ message: mensaje, duration: 2500, position: 'bottom' });
			await toast.present();
		}
	},
	ionViewWillEnter() {
		this.cargar();
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

.avatar-iniciales {
	background-color: var(--ion-color-primary);
	color: #fff;
	display: flex;
	align-items: center;
	justify-content: center;
	font-weight: bold;
	font-size: 1.1rem;
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

.badge-estado {
	text-transform: capitalize;
	padding: 4px 8px;
	font-size: 0.75rem;
	border-radius: 6px;
}

.info-row {
	display: flex;
	align-items: center;
	gap: 10px;
	margin-top: 8px;
	opacity: 0.9;
	font-size: 0.95rem;
}

.info-icon {
	font-size: 1.1rem;
	color: var(--ion-color-primary);
}

.acciones-row {
	display: flex;
	justify-content: flex-end;
	gap: 8px;
	border-top: 1px solid rgba(255, 255, 255, 0.08);
	padding-top: 8px;
}

.barra-volver {
	padding: 8px 16px 0;
}

.nota-constancia {
	display: block;
	text-align: center;
	padding: 8px;
}
</style>