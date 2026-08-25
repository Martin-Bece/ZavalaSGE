<template>
  <ion-page>
    <ion-header :translucent="true">
      <ion-toolbar>
        <ion-title>ZavalaSGE</ion-title>
        <!-- Toggle en la barra superior -->
        <ion-buttons slot="end">
          <ion-item lines="none" class="toggle-item">
            <ion-icon :icon="moonOutline" slot="start"></ion-icon>
            <ion-toggle
              :checked="app_store.tema_oscuro"
              @ionChange="app_store.alternar_tema()"
            ></ion-toggle>
          </ion-item>
        </ion-buttons>
      </ion-toolbar>
    </ion-header>

    <ion-content class="ion-padding">
      <ion-card>
        <ion-card-header>
          <ion-card-title>Bienvenido a ZavalaSGE</ion-card-title>
          <ion-card-subtitle>Sistema de Gestión Escolar - Escuela Nocturna</ion-card-subtitle>
        </ion-card-header>
        <ion-card-content>
          El proyecto está configurado correctamente en JavaScript puro con Pinia y modo oscuro persisitido.
        </ion-card-content>
      </ion-card>
    </ion-content>
  </ion-page>
</template>

<script>
import { 
  IonPage, 
  IonHeader, 
  IonToolbar, 
  IonTitle, 
  IonContent, 
  IonItem, 
  IonToggle,
  IonButtons,
  IonIcon,
  IonCard,
  IonCardHeader,
  IonCardTitle,
  IonCardSubtitle,
  IonCardContent
} from '@ionic/vue';
import { moonOutline } from 'ionicons/icons';
import { use_app_store } from '@/stores/app_store';

export default {
  name: 'HomePage',
  components: {
    IonPage, 
    IonHeader, 
    IonToolbar, 
    IonTitle, 
    IonContent, 
    IonItem, 
    IonToggle,
    IonButtons,
    IonIcon,
    IonCard,
    IonCardHeader,
    IonCardTitle,
    IonCardSubtitle,
    IonCardContent
  },
  data() {
    return {
      moonOutline
    }
  },
  computed: {
    app_store() {
      return use_app_store();
    }
  }
}
</script>

<style scoped>
#container {
  text-align: center;

  position: absolute;
  left: 0;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
}

#container strong {
  font-size: 20px;
  line-height: 26px;
}

#container p {
  font-size: 16px;
  line-height: 22px;

  color: #8c8c8c;

  margin: 0;
}

#container a {
  text-decoration: none;
}
</style>
