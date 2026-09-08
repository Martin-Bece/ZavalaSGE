<template>

    <comp-page

        :titulo="'Gestión de Personal'"

        :mostrar_actualizar="true"

        @actualizar="actualizarDatos"

    >

        <!-- 1. Cargando (Esqueleto) -->

        <div v-if="store.cargando" class="ion-padding">

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



        <!-- 2. Error -->

        <div v-else-if="store.error" class="ion-padding ion-text-center estado-container">

            <ion-icon :icon="alertCircleOutline" class="estado-icono color-danger" />

            <h3>Ocurrió un problema</h3>

            <p>{{ store.error }}</p>

            <ion-button fill="outline" color="danger" class="ion-margin-top" @click="actualizarDatos">

                <ion-icon slot="start" :icon="refreshOutline" />

                Reintentar

            </ion-button>

        </div>



        <!-- 3. Vacío (Sin registros) -->

        <div v-else-if="!store.hay_personas" class="ion-padding ion-text-center estado-container">

            <ion-icon :icon="peopleOutline" class="estado-icono color-muted" />

            <h3>Sin registros</h3>

            <p>No se encontraron personas en el sistema.</p>

        </div>



        <!-- 4. Con datos -->

        <div v-else class="ion-padding-horizontal ion-padding-top">

            <ion-card v-for="persona in store.personas" :key="persona.id" class="card-custom">

                <ion-item lines="none">

                    <ion-avatar slot="start" class="avatar-iniciales">

                        <span>{{ obtenerIniciales(persona.nombre, persona.apellido) }}</span>

                    </ion-avatar>

                    <ion-label class="ion-text-wrap">

                        <h2><strong>{{ persona.nombre }} {{ persona.apellido }}</strong></h2>

                        <ion-chip color="primary" class="chip-dni">

                            <ion-icon :icon="cardOutline" />

                            <ion-label>DNI: {{ persona.dni || 'Sin registrar' }}</ion-label>

                        </ion-chip>

                    </ion-label>

                </ion-item>



                <ion-card-content class="card-contenido">

                    <div class="info-row">

                        <ion-icon :icon="callOutline" class="info-icon" />

                        <span>{{ persona.telefono || 'Sin teléfono' }}</span>

                    </div>



                    <div class="info-row">

                        <ion-icon :icon="locationOutline" class="info-icon" />

                        <span v-if="persona.direccion">{{ persona.direccion }}</span>

                        <ion-badge v-else color="warning" mode="ios">Sin dirección registrada</ion-badge>

                    </div>

                </ion-card-content>

            </ion-card>

        </div>

    </comp-page>

</template>



<script>

import comp_page from '@/components/estructura/comp_page.vue';

import { personas_store, cargar_personas } from '@/stores/personas_store';

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

    callOutline,

    cardOutline,

    locationOutline,

    peopleOutline,

    refreshOutline

} from 'ionicons/icons';



export default {

    name: 'personal_page',

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

            alertCircleOutline,

            refreshOutline,

            peopleOutline,

            cardOutline,

            callOutline,

            locationOutline

        };

    },

    computed: {

        store() {

            return personas_store;

        }

    },

    methods: {

        actualizarDatos() {

            cargar_personas();

        },

        obtenerIniciales(nombre, apellido) {

            const n = nombre ? nombre.charAt(0) : '';

            const a = apellido ? apellido.charAt(0) : '';

            return `${n}${a}`.toUpperCase();

        }

    },

    mounted() {

        cargar_personas();

    }

};

</script>



<style scoped>

.card-custom {

    margin-bottom: 16px;

    border-radius: 14px;

    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.06);

}



.avatar-iniciales {

    background-color: var(--ion-color-primary);

    color: #fff;

    display: flex;

    align-items: center;

    justify-content: center;

    font-weight: bold;

    font-size: 1.1rem;

}



.chip-dni {

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

    color: var(--ion-color-primary);

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

