<template>
  <ion-app>
    <ion-router-outlet />
  </ion-app>
</template>

<script>
import { IonApp, IonRouterOutlet } from '@ionic/vue';
import { use_app_store } from './stores/app_store';

export default {
  name: 'App',
  components: {
    IonApp,
    IonRouterOutlet
  },
  mounted() {
    const store = use_app_store();
    store.aplicar_tema_guardado();
  }
};
</script>
