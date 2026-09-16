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

            <ion-list>
                <ion-list-header>Ajustes</ion-list-header>
                
                <!-- Toggle Modo Oscuro -->
                <ion-item lines="none">
                    <ion-icon slot="start" :icon="iconoTema" />
                    <ion-label>Modo oscuro</ion-label>
                    <ion-toggle
                        :checked="app_store.tema_oscuro"
                        @ionChange="app_store.alternar_tema()"
                    />
                </ion-item>

                <!-- Botón Cerrar Sesión con la misma estética -->
                <ion-menu-toggle :auto-hide="false">
                    <ion-item button :detail="false" @click="hacer_logout" lines="none" class="item-logout">
                        <ion-icon slot="start" :icon="logOutOutline" color="danger" />
                        <ion-label color="danger">Cerrar Sesión</ion-label>
                    </ion-item>
                </ion-menu-toggle>
            </ion-list>
        </ion-content>
    </ion-menu>
</template>

<script setup>
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import {
    IonContent, IonHeader, IonIcon, IonItem, IonLabel, 
    IonList, IonListHeader, IonMenu, IonMenuToggle, 
    IonTitle, IonToggle, IonToolbar
} from '@ionic/vue';
import { logOutOutline, moonOutline, sunnyOutline } from 'ionicons/icons';
import { obtener_grupos_menu } from '@/config/navegacion';
import { use_app_store } from '@/stores/app_store';
import { use_sesion_store } from '@/stores/sesion_store';

defineProps({
    content_id: {
        type: String,
        default: 'contenido_principal'
    }
});

const router = useRouter();
const app_store = use_app_store();
const sesion_store = use_sesion_store();

// Reacciona automáticamente si rol_activo cambia al cargar la app
const grupos_menu = computed(() => {
    return obtener_grupos_menu(sesion_store.rol_activo);
});

const iconoTema = computed(() => {
    return app_store.tema_oscuro ? sunnyOutline : moonOutline;
});

// Limpia la sesión y redirige de forma segura al login
async function hacer_logout() {
    await sesion_store.salir();
    router.replace('/login');
}
</script>

<style scoped>
.item-logout {
    cursor: pointer;
}
</style>