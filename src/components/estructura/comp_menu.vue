<template>
	<ion-menu :content-id="content_id" type="overlay">
		<ion-header>
			<ion-toolbar color="primary">
				<ion-title>ZavalaSGE</ion-title>
			</ion-toolbar>
		</ion-header>

		<ion-content>
			<ion-list v-for="grupo in grupos_menu" :key="grupo.id">
				<ion-list-header v-if="grupo.titulo">{{ grupo.titulo }}</ion-list-header>
				<ion-menu-toggle v-for="item in grupo.items" :key="item.id" :auto-hide="false">
					<ion-item button :detail="false" :router-link="item.ruta" router-direction="root">
						<ion-icon slot="start" :icon="item.icono" />
						<ion-label>{{ item.titulo }}</ion-label>
					</ion-item>
				</ion-menu-toggle>
			</ion-list>

			<!-- Toggle de Modo Oscuro con Pinia -->
			<ion-list>
				<ion-list-header>Ajustes</ion-list-header>
				<ion-item lines="none">
					<ion-icon slot="start" :icon="iconoTema" />
					<ion-label>Modo oscuro</ion-label>
					<ion-toggle
						:checked="app_store.tema_oscuro"
						@ionChange="app_store.alternar_tema()"
					/>
				</ion-item>
			</ion-list>
		</ion-content>
	</ion-menu>
</template>

<script>import {
	IonContent,
	IonHeader,
	IonIcon,
	IonItem,
	IonLabel,
	IonList,
	IonListHeader,
	IonMenu,
	IonMenuToggle,
	IonTitle,
	IonToggle,
	IonToolbar
} from '@ionic/vue';
import { moonOutline, sunnyOutline } from 'ionicons/icons';
import { obtener_grupos_menu } from '@/config/navegacion';
import { use_app_store } from '@/stores/app_store';

export default {
	name: 'comp_menu',
	components: {
		IonContent,
		IonHeader,
		IonIcon,
		IonItem,
		IonLabel,
		IonList,
		IonListHeader,
		IonMenu,
		IonMenuToggle,
		IonTitle,
		IonToggle,
		IonToolbar
	},
	props: {
		content_id: {
			type: String,
			required: true
		}
	},
	computed: {
		grupos_menu(){
			return obtener_grupos_menu();
		},
		app_store(){
			return use_app_store();
		},
		iconoTema(){
			return this.app_store.tema_oscuro ? sunnyOutline : moonOutline;
		}
	}
};</script>