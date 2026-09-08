<template>
    <comp-page :titulo="'Inicio'" :mostrar_actualizar="true">
        <!-- Banner de Bienvenida -->
        <div class="banner-bienvenida">
            <div class="banner-badge">
                <ion-badge color="success" mode="ios">
                    <ion-icon :icon="checkmarkCircleOutline" /> Sistema Operativo
                </ion-badge>
            </div>
            <span class="escuela-tag">ESCUELA N° 4</span>
            <h1>Bienvenido a ZavalaSGE</h1>
            <p>Sistema de Gestión Escolar cargado y listo para operar.</p>
        </div>

        <!-- Sección de Accesos Rápidos -->
        <div class="ion-padding-horizontal">
            <h2 class="seccion-titulo">Acceso Rápido</h2>

            <div class="grid-accesos">
                <!-- Mis Materias -->
                <div class="card-acceso" @click="$router.push('/materias')">
                    <div class="icon-wrapper color-primary">
                        <ion-icon :icon="bookOutline" />
                    </div>
                    <div class="acceso-info">
                        <h3>Mis Materias</h3>
                        <p>Gestión académica</p>
                    </div>
                </div>

                <!-- Personal / Personas -->
                <div class="card-acceso" @click="$router.push('/personal')">
                    <div class="icon-wrapper color-tertiary">
                        <ion-icon :icon="peopleOutline" />
                    </div>
                    <div class="acceso-info">
                        <h3>Personal</h3>
                        <p>Docentes y alumnos</p>
                    </div>
                </div>

                <!-- Tomar Asistencia -->
                <div class="card-acceso" @click="$router.push('/asistencia')">
                    <div class="icon-wrapper color-danger">
                        <ion-icon :icon="checkboxOutline" />
                    </div>
                    <div class="acceso-info">
                        <h3>Asistencia</h3>
                        <p>Control diario</p>
                    </div>
                </div>

                <!-- Cargar Notas -->
                <div class="card-acceso" @click="$router.push('/calificaciones')">
                    <div class="icon-wrapper color-warning">
                        <ion-icon :icon="createOutline" />
                    </div>
                    <div class="acceso-info">
                        <h3>Notas</h3>
                        <p>Cargar calificaciones</p>
                    </div>
                </div>
            </div>
        </div>
    </comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { IonBadge, IonIcon } from '@ionic/vue';
import {
    bookOutline,
    checkboxOutline,
    checkmarkCircleOutline,
    createOutline,
    peopleOutline
} from 'ionicons/icons';

export default {
    name: 'inicio_page',
    components: {
        CompPage: comp_page,
        IonBadge,
        IonIcon
    },
    setup() {
        return {
            bookOutline,
            checkboxOutline,
            checkmarkCircleOutline,
            createOutline,
            peopleOutline
        };
    }
};
</script>

<style scoped>
.banner-bienvenida {
    background: linear-gradient(135deg, var(--ion-color-primary) 0%, var(--ion-color-primary-shade) 100%);
    color: #ffffff;
    padding: 24px 20px;
    margin: 16px;
    border-radius: 20px;
    box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
    position: relative;
    overflow: hidden;
}

.banner-badge {
    margin-bottom: 12px;
}

.escuela-tag {
    font-size: 0.75rem;
    font-weight: 700;
    letter-spacing: 1px;
    opacity: 0.85;
    display: block;
}

.banner-bienvenida h1 {
    font-size: 1.5rem;
    font-weight: 800;
    margin: 4px 0 8px 0;
}

.banner-bienvenida p {
    font-size: 0.9rem;
    margin: 0;
    opacity: 0.9;
    line-height: 1.4;
}

.seccion-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    margin: 16px 0 12px 4px;
    color: var(--ion-color-dark);
}

.grid-accesos {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
    margin-bottom: 20px;
}

.card-acceso {
    background: var(--ion-card-background, #ffffff);
    border-radius: 16px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    border: 1px solid rgba(0, 0, 0, 0.04);
    transition: transform 0.2s ease, box-shadow 0.2s ease;
    cursor: pointer;
}

.card-acceso:active {
    transform: scale(0.97);
}

.icon-wrapper {
    width: 44px;
    height: 44px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 22px;
}

.color-primary {
    background: rgba(var(--ion-color-primary-rgb), 0.12);
    color: var(--ion-color-primary);
}

.color-tertiary {
    background: rgba(var(--ion-color-tertiary-rgb), 0.12);
    color: var(--ion-color-tertiary);
}

.color-danger {
    background: rgba(var(--ion-color-danger-rgb), 0.12);
    color: var(--ion-color-danger);
}

.color-warning {
    background: rgba(var(--ion-color-warning-rgb), 0.12);
    color: var(--ion-color-warning-shade);
}

.acceso-info h3 {
    font-size: 0.95rem;
    font-weight: 700;
    margin: 0 0 2px 0;
    color: var(--ion-color-dark);
}

.acceso-info p {
    font-size: 0.75rem;
    margin: 0;
    color: var(--ion-color-medium);
}
</style>