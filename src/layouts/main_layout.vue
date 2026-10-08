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
import { useRouter } from 'vue-router';
import { App } from '@capacitor/app';
import {
    IonPage,
    IonRouterOutlet,
    IonTabs
} from '@ionic/vue';
import comp_menu from '@/components/estructura/comp_menu.vue';
import comp_tabs from '@/components/estructura/comp_tabs.vue';
import { avisar_local } from '@/services/notificacion_local_service';
import { detener_push, iniciar_push } from '@/services/push_service';
import { use_notificaciones_store } from '@/stores/notificaciones_store';

const router = useRouter();
const notificaciones = use_notificaciones_store();
let escucha_app = null;

onMounted(async () => {
    notificaciones.iniciar_sondeo();
    // Al volver a la app desde segundo plano, el globo se actualiza ya mismo.
    escucha_app = await App.addListener('appStateChange', ({ isActive }) => {
        if (isActive) notificaciones.refrescar_contador();
    });

    // El push es opcional: si falla (sin permiso, sin google-services.json), lo demás sigue andando.
    const push = await iniciar_push({
        al_recibir: (n) => {
            avisar_local(n.title || 'ZavalaSGE', n.body || '');
            notificaciones.refrescar_contador();
        },
        al_tocar: () => router.push('/app/notificaciones')
    });
    if (push.mensaje) console.warn(push.mensaje);
});

onBeforeUnmount(() => {
    notificaciones.detener_sondeo();
    notificaciones.limpiar();
    detener_push();
    if (escucha_app) escucha_app.remove();
});
</script>