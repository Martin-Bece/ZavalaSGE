<template>
    <comp-page :titulo="'Inicio'" :mostrar_actualizar="true">
        <!-- Banner de Bienvenida -->
        <div class="banner-bienvenida">
            <div class="banner-badge">
                <ion-badge color="success" mode="ios">
                    <ion-icon :icon="checkmarkCircleOutline" /> Sistema Operativo
                </ion-badge>
            </div>
            <span class="escuela-tag">ESCUELA N° 4 · {{ etiqueta_rol }}</span>
            <h1>{{ saludo }}</h1>
            <p>{{ mensaje_rol }}</p>
        </div>

        <!-- Accesos rápidos: los que corresponden al rol -->
        <div class="ion-padding-horizontal">
            <h2 class="seccion-titulo">Acceso Rápido</h2>

            <div class="grid-accesos">
                <div
                    v-for="acceso in accesos"
                    :key="acceso.id"
                    class="card-acceso"
                    @click="$router.push(acceso.ruta)"
                >
                    <div class="icon-wrapper" :class="`color-${acceso.color}`">
                        <ion-icon :icon="acceso.icono" />
                    </div>
                    <div class="acceso-info">
                        <h3>{{ acceso.titulo }}</h3>
                        <p>{{ acceso.descripcion }}</p>
                    </div>
                </div>
            </div>

            <!-- Escáner de constancias: solo para los roles con permiso -->
            <template v-if="puede_escanear">
                <h2 class="seccion-titulo">Constancias</h2>
                <div class="bloque-escaner">
                    <div class="escaner-info">
                        <div class="icon-wrapper color-secondary">
                            <ion-icon :icon="qrCodeOutline" />
                        </div>
                        <p>Escaneá el QR de una constancia para abrir la ficha del alumno.</p>
                    </div>
                    <comp_boton_escanear expand="block" fill="solid" size="default" />
                </div>
            </template>
        </div>
    </comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import comp_boton_escanear from '@/components/dominio/comp_boton_escanear.vue';
import { navegacion } from '@/config/navegacion';
import { use_sesion_store } from '@/stores/sesion_store';
import { IonBadge, IonIcon } from '@ionic/vue';
import { checkmarkCircleOutline, qrCodeOutline } from 'ionicons/icons';

// Quién puede escanear el QR de una constancia. Agregá 'DOCENTE' si también debe poder.
const ROLES_ESCANER = ['ADMIN', 'PRECEPTOR', 'DOCENTE'];

// Pantallas que no tienen sentido como acceso rápido.
const EXCLUIDOS = ['inicio', 'perfil'];

// Color y descripción de cada acceso. La ruta, el título, el ícono y los roles salen de
// navegacion.js. Una descripción puede ser una función del rol.
const DETALLE = {
    personal: { color: 'tertiary', descripcion: 'Docentes y preceptores' },
    academico: { color: 'primary', descripcion: 'Gestión académica' },
    reportes: { color: 'warning', descripcion: 'Informes y listados' },
    asistencia: { color: 'danger', descripcion: 'Control diario' },
    comisiones: { color: 'tertiary', descripcion: 'Cursos y comisiones' },
    calificaciones: {
        color: 'warning',
        descripcion: (rol) => (rol === 'DOCENTE' ? 'Cargar calificaciones' : 'Consultar calificaciones')
    },
    materias: { color: 'primary', descripcion: 'Mis materias y alumnos' }
};

const ETIQUETAS_ROL = {
    ADMIN: 'ADMINISTRACIÓN',
    PRECEPTOR: 'PRECEPTORÍA',
    DOCENTE: 'DOCENCIA'
};

const MENSAJES_ROL = {
    ADMIN: 'Gestioná el personal, los alumnos y los reportes de la escuela.',
    PRECEPTOR: 'Seguí la asistencia y las calificaciones de tus comisiones.',
    DOCENTE: 'Consultá tus materias y cargá las calificaciones de tus alumnos.'
};

export default {
    name: 'inicio_page',
    components: {
        CompPage: comp_page,
        comp_boton_escanear,
        IonBadge,
        IonIcon
    },
    data() {
        return {
            checkmarkCircleOutline,
            qrCodeOutline
        };
    },
    computed: {
        sesion() {
            return use_sesion_store();
        },
        rol() {
            return this.sesion.rol_activo;
        },
        saludo() {
            const nombre = this.sesion.usuario && this.sesion.usuario.nombre;
            return nombre ? `Hola, ${nombre}` : 'Bienvenido a ZavalaSGE';
        },
        etiqueta_rol() {
            return ETIQUETAS_ROL[this.rol] || 'ZAVALASGE';
        },
        mensaje_rol() {
            return MENSAJES_ROL[this.rol] || 'Sistema de Gestión Escolar cargado y listo para operar.';
        },
        accesos() {
            const rol = this.rol;
            return navegacion
                .filter((item) => !EXCLUIDOS.includes(item.id))
                .filter((item) => !item.roles || item.roles.includes(rol))
                .map((item) => {
                    const detalle = DETALLE[item.id] || {};
                    const descripcion =
                        typeof detalle.descripcion === 'function'
                            ? detalle.descripcion(rol)
                            : detalle.descripcion || '';
                    return {
                        id: item.id,
                        titulo: item.titulo,
                        ruta: item.ruta,
                        icono: item.icono,
                        color: detalle.color || 'primary',
                        descripcion
                    };
                });
        },
        puede_escanear() {
            return ROLES_ESCANER.includes(this.rol);
        }
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
    flex-shrink: 0;
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

.color-secondary {
    background: rgba(var(--ion-color-secondary-rgb), 0.12);
    color: var(--ion-color-secondary);
}

.color-tertiary {
    background: rgba(var(--ion-color-tertiary-rgb), 0.12);
    color: var(--ion-color-tertiary);
}

.color-success {
    background: rgba(var(--ion-color-success-rgb), 0.12);
    color: var(--ion-color-success);
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

.bloque-escaner {
    background: var(--ion-card-background, #ffffff);
    border-radius: 16px;
    padding: 16px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    border: 1px solid rgba(0, 0, 0, 0.04);
    margin-bottom: 24px;
}

.escaner-info {
    display: flex;
    align-items: center;
    gap: 12px;
    margin-bottom: 12px;
}

.escaner-info p {
    margin: 0;
    font-size: 0.9rem;
    color: var(--ion-color-medium-shade);
    line-height: 1.4;
}
</style>