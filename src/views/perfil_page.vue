<template>
    <comp-page :titulo="'Mi Perfil'">
        <div class="ion-padding-horizontal ion-padding-top">
            <!-- 1. Tarjeta de Usuario / Configuración -->
            <ion-card class="card-custom">
                <ion-item lines="none" class="item-perfil">
                    <ion-avatar slot="start" class="avatar-perfil">
                        <ion-icon :icon="personCircleOutline" />
                    </ion-avatar>
                    <ion-label class="ion-text-wrap">
                        <h2><strong>Perfil de Usuario</strong></h2>
                        <p>Información personal y preferencias de la cuenta.</p>
                    </ion-label>
                </ion-item>

                <ion-card-content class="card-acciones">
                    <ion-button color="primary" size="small" fill="outline" class="btn-editar">
                        <ion-icon slot="start" :icon="createOutline" />
                        Editar Datos
                    </ion-button>
                </ion-card-content>
            </ion-card>

            <!-- 2. Panel de Diagnóstico de Red -->
            <ion-card class="card-custom ion-margin-top">
                <ion-card-header>
                    <ion-card-subtitle class="header-diagnostico">
                        <ion-icon :icon="hardwareChipOutline" />
                        Diagnóstico de Red
                    </ion-card-subtitle>
                </ion-card-header>

                <ion-card-content class="contenido-diagnostico">
                    <!-- URL de la API -->
                    <ion-item lines="full" class="item-diagnostico">
                        <ion-icon slot="start" :icon="globeOutline" class="icono-diagnostico" />
                        <ion-label class="ion-text-wrap">
                            <h3>URL de la API</h3>
                            <p class="texto-url">{{ api_url || 'Sin configurar' }}</p>
                        </ion-label>
                    </ion-item>

                    <!-- Estado de Conexión -->
                    <ion-item lines="none" class="item-diagnostico">
                        <ion-icon slot="start" :icon="pulseOutline" class="icono-diagnostico" />
                        <ion-label class="ion-text-wrap">
                            <h3>Estado de Conexión</h3>
                            <p>
                                <ion-badge :color="badgeColor" mode="ios" class="badge-estado">
                                    {{ mensaje }}
                                </ion-badge>
                            </p>
                        </ion-label>
                        <ion-button 
                            slot="end" 
                            fill="solid"
                            color="primary"
                            size="small"
                            :disabled="estado === 'probando'" 
                            @click="probar"
                        >
                            <ion-icon slot="start" :icon="refreshOutline" />
                            Probar
                        </ion-button>
                    </ion-item>
                </ion-card-content>
            </ion-card>
        </div>
    </comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_api_url } from '@/config/debug';
import { consultar_health } from '@/services/health_service';
import { 
    IonAvatar,
    IonBadge,
    IonButton, 
    IonCard, 
    IonCardContent, 
    IonCardHeader, 
    IonCardSubtitle, 
    IonIcon,
    IonItem, 
    IonLabel 
} from '@ionic/vue';
import { 
    createOutline, 
    globeOutline, 
    hardwareChipOutline, 
    personCircleOutline, 
    pulseOutline, 
    refreshOutline 
} from 'ionicons/icons';

export default {
    name: 'perfil_page',
    components: {
        CompPage: comp_page,
        IonCard,
        IonCardHeader,
        IonCardSubtitle,
        IonCardContent,
        IonButton,
        IonItem,
        IonLabel,
        IonAvatar,
        IonBadge,
        IonIcon
    },
    data() {
        return {
            api_url: obtener_api_url(),
            estado: 'sin_probar',
            mensaje: 'Sin probar',
            personCircleOutline,
            createOutline,
            hardwareChipOutline,
            globeOutline,
            pulseOutline,
            refreshOutline
        };
    },
    computed: {
        badgeColor() {
            if (this.estado === 'ok') return 'success';
            if (this.estado === 'error') return 'danger';
            if (this.estado === 'probando') return 'warning';
            return 'medium';
        }
    },
    methods: {
        async probar() {
            this.estado = 'probando';
            this.mensaje = 'Probando conexión...';
            try {
                const respuesta = await consultar_health();
                this.estado = 'ok';
                this.mensaje = `API Conectada (${respuesta.status || 'OK'})`;
            } catch (error) {
                this.estado = 'error';
                this.mensaje = error.mensaje;
            }
        }
    }
};
</script>

<style scoped>
.card-custom {
    border-radius: 14px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
    margin-left: 0;
    margin-right: 0;
}

.item-perfil {
    --padding-start: 12px;
    --inner-padding-end: 12px;
    margin-top: 6px;
}

.avatar-perfil {
    width: 50px;
    height: 50px;
    background-color: var(--ion-color-light);
    color: var(--ion-color-primary);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 2.2rem;
}

.card-acciones {
    padding-top: 0;
    display: flex;
    justify-content: flex-end;
}

.btn-editar {
    --border-radius: 8px;
    font-weight: 600;
}

.header-diagnostico {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 0.85rem;
    font-weight: bold;
    color: var(--ion-color-primary);
    letter-spacing: 0.5px;
}

.contenido-diagnostico {
    padding-top: 0;
    padding-bottom: 8px;
}

.item-diagnostico {
    --padding-start: 0;
    --inner-padding-end: 0;
}

.icono-diagnostico {
    font-size: 1.3rem;
    color: var(--ion-color-medium);
    margin-right: 12px;
}

.texto-url {
    font-family: monospace;
    font-size: 0.9rem;
    color: var(--ion-color-dark);
}

.badge-estado {
    margin-top: 4px;
    padding: 6px 10px;
    font-size: 0.8rem;
    border-radius: 6px;
}
</style>