<template>
	<ion-list>
		<ion-item v-for="i in cantidad" :key="i">
			<ion-avatar v-if="mostrar_icono" slot="start">
				<ion-skeleton-text :animated="true" />
			</ion-avatar>
			<ion-label>
				<h3>
					<ion-skeleton-text :animated="true" style="width: 60%;" />
				</h3>
				<p>
					<ion-skeleton-text :animated="true" style="width: 80%;" />
				</p>
			</ion-label>
		</ion-item>
	</ion-list>
</template>

<script>import {
	IonAvatar,
	IonItem,
	IonLabel,
	IonList,
	IonSkeletonText
} from '@ionic/vue';

export default {
	name: 'comp_esqueleto',
	components: {
		IonAvatar,
		IonItem,
		IonLabel,
		IonList,
		IonSkeletonText
	},
	props: {
		cantidad: {
			type: Number,
			default: 5
		},
		mostrar_icono: {
			type: Boolean,
			default: true
		}
	}
};</script>