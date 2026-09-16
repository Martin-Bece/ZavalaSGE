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
import {
    IonPage,
    IonRouterOutlet,
    IonTabs
} from '@ionic/vue';
import comp_menu from '@/components/estructura/comp_menu.vue';
import comp_tabs from '@/components/estructura/comp_tabs.vue';
</script>