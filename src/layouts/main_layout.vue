<template>
  <ion-page>
    <!-- Usar guion medio: content-id -->
    <comp_menu content-id="contenido_principal" />

    <ion-tabs id="contenido_principal">
      <ion-router-outlet />
      <comp_tabs />
    </ion-tabs>
  </ion-page>
</template>

<script setup>
import { onBeforeUnmount, onMounted } from 'vue';
import { App } from '@capacitor/app';
import {
    IonPage,
    IonRouterOutlet,
    IonTabs
} from '@ionic/vue';
import comp_menu from '@/components/estructura/comp_menu.vue';
import comp_tabs from '@/components/estructura/comp_tabs.vue';
import { use_notificaciones_store } from '@/stores/notificaciones_store';

const notificaciones = use_notificaciones_store();
let escucha_app = null;

onMounted(async () => {
    notificaciones.iniciar_sondeo();
    // Al volver a la app desde segundo plano, el globo se actualiza ya mismo.
    escucha_app = await App.addListener('appStateChange', ({ isActive }) => {
        if (isActive) notificaciones.refrescar_contador();
    });
});

onBeforeUnmount(() => {
    notificaciones.detener_sondeo();
    notificaciones.limpiar();
    if (escucha_app) escucha_app.remove();
});
</script>