<template>
    <comp-page 
        :titulo="'Comisiones'" 
        :mostrar_actualizar="true" 
        @actualizar="cargarDatos"
    >
        <!-- 1. Estado Cargando (Esqueleto) -->
        <div v-if="cargando" class="ion-padding">
            <ion-card v-for="i in 3" :key="i" class="card-custom">
                <ion-item lines="none">
                    <ion-avatar slot="start">
                        <ion-skeleton-text animated />
                    </ion-avatar>
                    <ion-label>
                        <ion-skeleton-text animated style="width: 60%; height: 20px;" />
                        <ion-skeleton-text animated style="width: 40%; height: 14px; margin-top: 6px;" />
                    </ion-label>
                </ion-item>
            </ion-card>
        </div>

        <!-- 2. Estado Error -->
        <div v-else-if="error" class="ion-padding ion-text-center estado-container">
            <ion-icon :icon="alertCircleOutline" class="estado-icono color-danger" />
            <h3>Ocurrió un problema</h3>
            <p>{{ error }}</p>
            <ion-button fill="outline" color="danger" class="ion-margin-top" @click="cargarDatos">
                <ion-icon slot="start" :icon="refreshOutline" />
                Reintentar
            </ion-button>
        </div>

        <!-- 3. Estado Vacío (Sin registros) -->
        <div v-else-if="listaComisiones.length === 0" class="ion-padding ion-text-center estado-container">
            <ion-icon :icon="layersOutline" class="estado-icono color-muted" />
            <h3>Sin comisiones registradas</h3>
            <p>No se encontraron comisiones o cursos disponibles.</p>
        </div>

        <!-- 4. Estado Con Datos -->
        <div v-else class="ion-padding-horizontal ion-padding-top">
            <ion-card v-for="comision in listaComisiones" :key="comision.id" class="card-custom">
                <ion-item lines="none">
                    <ion-avatar slot="start" class="avatar-icono">
                        <ion-icon :icon="layersOutline" />
                    </ion-avatar>
                    <ion-label class="ion-text-wrap">
                        <h2><strong>{{ comision.nombre }}</strong></h2>
                        <ion-chip v-if="comision.curso" color="tertiary" class="chip-curso">
                            <ion-icon :icon="ribbonOutline" />
                            <ion-label>{{ comision.curso }}</ion-label>
                        </ion-chip>
                    </ion-label>
                </ion-item>

                <ion-card-content class="card-contenido">
                    <div class="info-row">
                        <ion-icon :icon="personOutline" class="info-icon" />
                        <span v-if="comision.preceptor">Preceptor: <strong>{{ comision.preceptor }}</strong></span>
                        <ion-badge v-else color="warning" mode="ios">Sin preceptor asignado</ion-badge>
                    </div>
                </ion-card-content>
            </ion-card>
        </div>
    </comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_comisiones } from '@/datos/comisiones';
import {
    IonAvatar,
    IonBadge,
    IonButton,
    IonCard,
    IonCardContent,
    IonChip,
    IonIcon,
    IonItem,
    IonLabel,
    IonSkeletonText
} from '@ionic/vue';
import { 
    alertCircleOutline, 
    layersOutline, 
    personOutline, 
    refreshOutline, 
    ribbonOutline 
} from 'ionicons/icons';

export default {
    name: 'comisiones_page',
    components: {
        CompPage: comp_page,
        IonButton,
        IonCard,
        IonCardContent,
        IonSkeletonText,
        IonBadge,
        IonAvatar,
        IonItem,
        IonLabel,
        IonChip,
        IonIcon
    },
    data() {
        return {
            listaComisiones: [],
            cargando: true,
            error: null,
            alertCircleOutline,
            refreshOutline,
            layersOutline,
            personOutline,
            ribbonOutline
        };
    },
    methods: {
        async cargarDatos() {
            this.cargando = true;
            this.error = null;
            try {
                this.listaComisiones = await obtener_comisiones();
            } catch (err) {
                this.error = "Error al obtener las comisiones.";
            } finally {
                this.cargando = false;
            }
        }
    },
    mounted() {
        this.cargarDatos();
    }
};
</script>

<style scoped>
.card-custom {
    margin-bottom: 16px;
    border-radius: 14px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);
}

.avatar-icono {
    background-color: var(--ion-color-tertiary);
    color: #fff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.3rem;
}

.chip-curso {
    margin-left: 0;
    margin-top: 6px;
    height: 24px;
    font-size: 0.8rem;
}

.card-contenido {
    padding-top: 0;
}

.info-row {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-top: 8px;
    color: var(--ion-color-medium-shade);
    font-size: 0.95rem;
}

.info-icon {
    font-size: 1.1rem;
    color: var(--ion-color-tertiary);
}

.estado-container {
    margin-top: 40px;
}

.estado-icono {
    font-size: 64px;
}

.color-danger {
    color: var(--ion-color-danger);
}

.color-muted {
    color: var(--ion-color-medium);
}
</style>