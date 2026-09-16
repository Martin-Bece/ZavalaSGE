<script setup>
import { onMounted } from 'vue';
import { IonApp, IonRouterOutlet } from '@ionic/vue';
import { use_sesion_store } from '@/stores/sesion_store';

const sesion_store = use_sesion_store();

onMounted(async () => {
    await sesion_store.preparar();
    await sesion_store.iniciar();
});
</script>

<template>
    <ion-app>
        <ion-router-outlet />
    </ion-app>
</template>