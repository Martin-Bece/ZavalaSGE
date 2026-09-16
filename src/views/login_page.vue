<template>
  <ion-page>
    <ion-content class="ion-padding login-content" fullscreen>
      <div class="login-wrapper">
        <div class="login-card">
          
          <!-- Encabezado / Logo -->
          <div class="login-header">
            <div class="logo-circle">
              <ion-icon :icon="schoolOutline" class="logo-icon"></ion-icon>
            </div>
            <h1>ZavalaSGE</h1>
            <p>Sistema de Gestión Escolar</p>
          </div>

          <!-- Formulario -->
          <form @submit.prevent="al_enviar">
            <ion-list lines="none" class="form-list">
              
              <!-- Input Email -->
              <ion-item class="input-item">
                <ion-input
                  v-model="email"
                  type="email"
                  label="Correo Electrónico"
                  label-placement="stacked"
                  placeholder="usuario@zavala.edu.ar"
                  required
                  clear-input
                ></ion-input>
              </ion-item>

              <!-- Input Contraseña -->
              <ion-item class="input-item">
                <ion-input
                  v-model="password"
                  type="password"
                  label="Contraseña"
                  label-placement="stacked"
                  placeholder="••••••••"
                  required
                ></ion-input>
              </ion-item>
            </ion-list>

            <!-- Mensaje de Error -->
            <div v-if="sesion_store.error" class="mensaje-error-container">
              <ion-icon :icon="alertCircleOutline" color="danger"></ion-icon>
              <p class="mensaje-error">{{ sesion_store.error }}</p>
            </div>

            <!-- Botón de Ingreso -->
            <ion-button 
              type="submit" 
              expand="block" 
              class="btn-ingresar" 
              :disabled="sesion_store.cargando"
            >
              <ion-spinner v-if="sesion_store.cargando" name="crescent"></ion-spinner>
              <span v-else>Iniciar Sesión</span>
            </ion-button>
          </form>

        </div>
      </div>
    </ion-content>
  </ion-page>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { 
  IonPage, 
  IonContent, 
  IonList, 
  IonItem, 
  IonInput, 
  IonButton, 
  IonIcon, 
  IonSpinner 
} from '@ionic/vue';
import { schoolOutline, alertCircleOutline } from 'ionicons/icons';
import { use_sesion_store } from '@/stores/sesion_store';

const router = useRouter();
const sesion_store = use_sesion_store();

const email = ref('');
const password = ref('');

async function al_enviar() {
  const exito = await sesion_store.entrar(email.value, password.value);
  if (exito) {
    router.replace('/app/inicio');
  }
}
</script>

<style scoped>
/* Contenedor general centrado en pantalla */
.login-content {
  --background: var(--ion-background-color);
}

.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100%;
  padding: 16px;
}

/* Tarjeta contenedora con soporte automático de modo oscuro */
.login-card {
  width: 100%;
  max-width: 420px;
  background: var(--ion-card-background, #ffffff);
  padding: 32px 24px;
  border-radius: 16px;
  box-shadow: 0 10px 25px rgba(0, 71, 171, 0.08);
  border: 1px solid rgba(0, 71, 171, 0.08);
}

/* Encabezado del login */
.login-header {
  text-align: center;
  margin-bottom: 28px;
}

.logo-circle {
  width: 64px;
  height: 64px;
  background: rgba(var(--ion-color-primary-rgb), 0.1);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 16px auto;
}

.logo-icon {
  font-size: 32px;
  color: var(--ion-color-primary);
}

.login-header h1 {
  font-size: 26px;
  font-weight: 700;
  color: var(--ion-text-color);
  margin: 0 0 4px 0;
}

.login-header p {
  font-size: 14px;
  color: var(--ion-color-secondary);
  margin: 0;
  font-weight: 500;
}

/* Listas y campos de entrada Ionic */
.form-list {
  background: transparent !important;
  margin-bottom: 20px;
  padding: 0 !important;
}

.input-item {
  --background: var(--ion-background-color);
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 10px;
  margin-bottom: 16px;
  --padding-start: 12px;
  --padding-end: 12px;
  --highlight-color-focused: var(--ion-color-primary);
}

body.dark .input-item {
  border: 1px solid rgba(255, 255, 255, 0.1);
}

/* Estilo de errores */
.mensaje-error-container {
  display: flex;
  align-items: center;
  gap: 8px;
  background: rgba(var(--ion-color-danger-rgb), 0.1);
  padding: 10px 14px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.mensaje-error {
  color: var(--ion-color-danger);
  font-size: 0.88rem;
  margin: 0;
  font-weight: 500;
}

/* Botón principal */
.btn-ingresar {
  --border-radius: 10px;
  font-weight: 600;
  letter-spacing: 0.5px;
  height: 48px;
  --box-shadow: 0 4px 12px rgba(0, 71, 171, 0.25);
}
</style>