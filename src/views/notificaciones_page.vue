<template>
	<comp_page
		titulo="Notificaciones"
		:mostrar_campana="false"
		:mostrar_actualizar="true"
		@actualizar="store.cargar()"
	>
		<div v-if="store.cargando && !store.hay_notificaciones">
			<ion-card v-for="i in 3" :key="i" class="card-custom">
				<ion-item lines="none">
					<ion-label>
						<ion-skeleton-text animated style="width: 60%; height: 18px" />
						<ion-skeleton-text animated style="width: 85%; height: 14px; margin-top: 6px" />
					</ion-label>
				</ion-item>
			</ion-card>
		</div>

		<comp_estado_error
			v-else-if="store.error"
			:mensaje="store.error"
			@reintentar="store.cargar()"
		/>

		<comp_estado_vacio
			v-else-if="!store.hay_notificaciones"
			titulo="Sin notificaciones"
			mensaje="Cuando pase algo que te interese, lo vas a ver acá."
		/>

		<template v-else>
			<ion-button
				v-if="store.no_leidas > 0"
				expand="block"
				fill="outline"
				class="boton-todas"
				@click="marcar_todas"
			>
				<ion-icon slot="start" :icon="checkmarkDoneOutline" />
				Marcar todas como leídas ({{ store.no_leidas }})
			</ion-button>

			<ion-list class="lista">
				<ion-item
					v-for="n in store.notificaciones"
					:key="n.id"
					button
					:detail="false"
					lines="full"
					:class="{ 'no-leida': !n.leida }"
					@click="abrir(n)"
				>
					<ion-icon slot="start" :icon="notificationsOutline" color="primary" />
					<ion-label class="ion-text-wrap">
						<h3>{{ n.titulo }}</h3>
						<p>{{ n.mensaje }}</p>
						<p class="fecha">{{ formatear_fecha(n.creada) }}</p>
					</ion-label>
					<ion-badge v-if="!n.leida" slot="end" color="danger" mode="ios">nueva</ion-badge>
				</ion-item>
			</ion-list>
		</template>
	</comp_page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import comp_estado_error from '@/components/base/comp_estado_error.vue';
import comp_estado_vacio from '@/components/base/comp_estado_vacio.vue';

import { use_notificaciones_store } from '@/stores/notificaciones_store';
import {
	IonBadge,
	IonButton,
	IonCard,
	IonIcon,
	IonItem,
	IonLabel,
	IonList,
	IonSkeletonText,
	toastController
} from '@ionic/vue';
import { checkmarkDoneOutline, notificationsOutline } from 'ionicons/icons';

export default {
	name: 'notificaciones_page',
	components: {
		comp_page,
		comp_estado_error,
		comp_estado_vacio,
		IonBadge,
		IonButton,
		IonCard,
		IonIcon,
		IonItem,
		IonLabel,
		IonList,
		IonSkeletonText
	},
	data() {
		return {
			checkmarkDoneOutline,
			notificationsOutline
		};
	},
	computed: {
		store() {
			return use_notificaciones_store();
		}
	},
	methods: {
		async abrir(notificacion) {
			if (notificacion.leida) return;
			const ok = await this.store.marcar_leida(notificacion.id);
			if (!ok) await this.avisar('No se pudo marcar como leída.');
		},
		async marcar_todas() {
			const ok = await this.store.marcar_todas();
			if (!ok) await this.avisar('No se pudieron marcar como leídas.');
		},
		// La API manda la fecha en UTC; acá se muestra en la hora del teléfono (por ejemplo, 21/9, 21:11).
		formatear_fecha(valor) {
			return new Date(valor).toLocaleString('es-AR', {
				day: 'numeric',
				month: 'numeric',
				hour: '2-digit',
				minute: '2-digit'
			});
		},
		async avisar(mensaje) {
			const toast = await toastController.create({ message: mensaje, duration: 2500, position: 'bottom' });
			await toast.present();
		}
	},
	ionViewDidEnter() {
		this.store.cargar();
	}
};
</script>

<style scoped>
.card-custom {
	margin: 12px 0;
	border-radius: 14px;
	background: var(--ion-card-background, #ffffff);
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

body.dark .card-custom {
	background: #1e293b;
}

.boton-todas {
	margin: 4px 0 12px;
}

.lista {
	border-radius: 14px;
	overflow: hidden;
}

.no-leida h3 {
	font-weight: 700;
}

.fecha {
	opacity: 0.6;
	font-size: 0.75rem;
}
</style>