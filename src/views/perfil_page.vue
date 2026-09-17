<template>
    <comp-page :titulo="'Mi Perfil'">
        <div class="ion-padding-horizontal ion-padding-top perfil-container">
            
            <!-- 1. Tarjeta de Usuario / Configuración -->
            <ion-card class="card-custom">
                <ion-item lines="none" class="item-perfil">
                    <ion-avatar slot="start" class="avatar-perfil">
                        <span v-if="inicialesUsuario" class="avatar-iniciales">{{ inicialesUsuario }}</span>
                        <ion-icon v-else :icon="personCircleOutline" />
                    </ion-avatar>
                    <ion-label class="ion-text-wrap">
                        <h2><strong>{{ nombreCompleto }}</strong></h2>
                        <p>{{ emailUsuario }}</p>
                        <p class="rol-badge-text"><ion-text color="primary">Rol: <strong>{{ rolUsuario }}</strong></ion-text></p>
                    </ion-label>
                </ion-item>

                <ion-card-content class="card-acciones">
                    <ion-button color="primary" size="small" fill="outline" class="btn-editar" @click="abrirModalEdicion">
                        <ion-icon slot="start" :icon="createOutline" />
                        Editar Cuenta y Foto
                    </ion-button>
                </ion-card-content>
            </ion-card>

            <!-- 2. Módulo de Cambio de Contraseña -->
            <ion-card class="card-custom ion-margin-top">
                <ion-card-header>
                    <ion-card-subtitle class="header-seccion">
                        <ion-icon :icon="lockClosedOutline" />
                        Seguridad y Contraseña
                    </ion-card-subtitle>
                </ion-card-header>

                <ion-card-content>
                    <form @submit.prevent="cambiarPassword">
                        <ion-item class="input-item" lines="full">
                            <ion-input
                                v-model="passForm.actual"
                                type="password"
                                label="Contraseña Actual"
                                label-placement="stacked"
                                placeholder="••••••••"
                                required
                            ></ion-input>
                        </ion-item>

                        <ion-item class="input-item" lines="full">
                            <ion-input
                                v-model="passForm.nueva"
                                type="password"
                                label="Nueva Contraseña"
                                label-placement="stacked"
                                placeholder="••••••••"
                                required
                            ></ion-input>
                        </ion-item>

                        <ion-item class="input-item" lines="none">
                            <ion-input
                                v-model="passForm.confirmar"
                                type="password"
                                label="Confirmar Nueva Contraseña"
                                label-placement="stacked"
                                placeholder="••••••••"
                                required
                            ></ion-input>
                        </ion-item>

                        <ion-button type="submit" expand="block" color="secondary" size="small" class="ion-margin-top btn-accion" :disabled="cargandoPassword">
                            <ion-spinner v-if="cargandoPassword" name="crescent" slot="start"></ion-spinner>
                            <ion-icon v-else slot="start" :icon="shieldCheckmarkOutline" />
                            {{ cargandoPassword ? 'Actualizando...' : 'Actualizar Contraseña' }}
                        </ion-button>
                    </form>
                </ion-card-content>
            </ion-card>

            <!-- 3. Panel de Diagnóstico de Red -->
            <ion-card class="card-custom ion-margin-top ion-margin-bottom">
                <ion-card-header>
                    <ion-card-subtitle class="header-diagnostico">
                        <ion-icon :icon="hardwareChipOutline" />
                        Diagnóstico de Red
                    </ion-card-subtitle>
                </ion-card-header>

                <ion-card-content class="contenido-diagnostico">
                    <ion-item lines="full" class="item-diagnostico">
                        <ion-icon slot="start" :icon="globeOutline" class="icono-diagnostico" />
                        <ion-label class="ion-text-wrap">
                            <h3>URL de la API</h3>
                            <p class="texto-url">{{ api_url || 'Sin configurar' }}</p>
                        </ion-label>
                    </ion-item>

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

        <!-- MODAL REDISEÑADO: EDITAR EMAIL Y FOTO -->
        <ion-modal :is-open="modalEdicionAbierto" @didDismiss="cerrarModalEdicion" class="modal-perfil">
            <template v-slot:default>
                <ion-header>
                    <ion-toolbar color="primary">
                        <ion-title>Configuración de Cuenta</ion-title>
                        <ion-buttons slot="end">
                            <ion-button @click="cerrarModalEdicion">Cerrar</ion-button>
                        </ion-buttons>
                    </ion-toolbar>
                </ion-header>

                <ion-content class="ion-padding">
                    <div class="modal-body-container">
                        
                        <!-- Sección de Foto de Perfil -->
                        <div class="profile-photo-section">
                            <ion-avatar class="modal-avatar-preview">
                                <span v-if="inicialesUsuario" class="avatar-iniciales">{{ inicialesUsuario }}</span>
                                <ion-icon v-else :icon="personCircleOutline" />
                            </ion-avatar>
                            <div class="photo-actions">
                                <h3>Foto de Perfil</h3>
                                <p>Personaliza tu avatar institucional</p>
                                <ion-button fill="outline" size="small" color="primary" class="ion-margin-top" @click="alertaFotoProximamente">
                                    <ion-icon slot="start" :icon="cameraOutline" />
                                    Cambiar Foto (Próximamente)
                                </ion-button>
                            </div>
                        </div>

                        <ion-item-divider class="divider-custom"></ion-item-divider>

                        <!-- Datos Institucionales (Solo lectura) -->
                        <div class="seccion-bloqueada-info">
                            <p class="info-label-bloqueada">Datos personales (Gestionados desde administración)</p>
                            
                            <ion-item class="input-item-bloqueado" lines="none">
                                <ion-input
                                    v-model="formEdit.nombre"
                                    label="Nombre"
                                    label-placement="stacked"
                                    readonly
                                ></ion-input>
                            </ion-item>

                            <ion-item class="input-item-bloqueado" lines="none">
                                <ion-input
                                    v-model="formEdit.apellido"
                                    label="Apellido"
                                    label-placement="stacked"
                                    readonly
                                ></ion-input>
                            </ion-item>
                        </div>

                        <!-- Formulario de edición editable (Solo Email) -->
                        <form @submit.prevent="guardarDatosEmail">
                            <ion-item class="input-item" lines="none">
                                <ion-input
                                    v-model="formEdit.email"
                                    type="email"
                                    label="Correo Electrónico de Contacto"
                                    label-placement="stacked"
                                    placeholder="usuario@zavala.edu.ar"
                                    required
                                ></ion-input>
                            </ion-item>
                            <p class="input-hint">El correo electrónico se utilizará para notificaciones del sistema.</p>

                            <ion-button type="submit" expand="block" class="ion-margin-top btn-accion" :disabled="cargandoEmail">
                                <ion-spinner v-if="cargandoEmail" name="crescent" slot="start"></ion-spinner>
                                <ion-icon v-else slot="start" :icon="saveOutline" />
                                {{ cargandoEmail ? 'Guardando...' : 'Guardar Correo' }}
                            </ion-button>
                        </form>

                    </div>
                </ion-content>
            </template>
        </ion-modal>

        <!-- Toast de Notificaciones -->
        <ion-toast
            :is-open="toast.abierto"
            :message="toast.mensaje"
            :duration="3000"
            :color="toast.color"
            position="bottom"
            @didDismiss="toast.abierto = false"
        ></ion-toast>

    </comp-page>
</template>

<script>
import comp_page from '@/components/estructura/comp_page.vue';
import { obtener_api_url } from '@/config/debug';
import { consultar_health } from '@/services/health_service';
import { use_sesion_store } from '@/stores/sesion_store';
import { actualizar_usuario, actualizar_password } from '@/services/usuarios_service';
import { 
    IonAvatar,
    IonBadge,
    IonButton,
    IonButtons, 
    IonCard, 
    IonCardContent, 
    IonCardHeader, 
    IonCardSubtitle, 
    IonContent,
    IonHeader,
    IonIcon,
    IonInput,
    IonItem, 
    IonItemDivider,
    IonLabel,
    IonModal,
    IonSpinner,
    IonText,
    IonTitle,
    IonToast,
    IonToolbar
} from '@ionic/vue';
import { 
    createOutline, 
    globeOutline, 
    hardwareChipOutline, 
    personCircleOutline, 
    pulseOutline, 
    refreshOutline,
    lockClosedOutline,
    shieldCheckmarkOutline,
    cameraOutline,
    saveOutline
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
        IonButtons,
        IonItem,
        IonItemDivider,
        IonInput,
        IonAvatar,
        IonBadge,
        IonIcon,
        IonModal,
        IonContent,
        IonHeader,
        IonToolbar,
        IonTitle,
        IonText,
        IonSpinner,
        IonToast,
        IonLabel
    },
    data() {
        return {
            api_url: obtener_api_url(),
            estado: 'sin_probar',
            mensaje: 'Sin probar',
            modalEdicionAbierto: false,
            cargandoEmail: false,
            cargandoPassword: false,
            
            formEdit: {
                nombre: '',
                apellido: '',
                email: ''
            },
            passForm: {
                actual: '',
                nueva: '',
                confirmar: ''
            },
            toast: {
                abierto: false,
                mensaje: '',
                color: 'success'
            },

            personCircleOutline,
            createOutline,
            hardwareChipOutline,
            globeOutline,
            pulseOutline,
            refreshOutline,
            lockClosedOutline,
            shieldCheckmarkOutline,
            cameraOutline,
            saveOutline
        };
    },
    computed: {
        sesionStore() {
            return use_sesion_store();
        },
        usuarioActual() {
            return this.sesionStore.usuario || {};
        },
        nombreCompleto() {
            const nombre = this.usuarioActual.nombre || '';
            const apellido = this.usuarioActual.apellido || '';
            return `${nombre} ${apellido}`.trim() || 'Usuario SGE';
        },
        emailUsuario() {
            return this.usuarioActual.email || 'Sin correo asignado';
        },
        rolUsuario() {
            return this.sesionStore.rol_activo || 'Invitado';
        },
        inicialesUsuario() {
            const nombre = this.usuarioActual.nombre || '';
            const apellido = this.usuarioActual.apellido || '';
            const n1 = nombre.charAt(0).toUpperCase();
            const n2 = apellido.charAt(0).toUpperCase();
            if (!n1 && !n2) return '';
            return `${n1}${n2}`;
        },
        badgeColor() {
            if (this.estado === 'ok') return 'success';
            if (this.estado === 'error') return 'danger';
            if (this.estado === 'probando') return 'warning';
            return 'medium';
        }
    },
    methods: {
        mostrarToast(mensaje, color = 'success') {
            this.toast.mensaje = mensaje;
            this.toast.color = color;
            this.toast.abierto = true;
        },
        async probar() {
            this.estado = 'probando';
            this.mensaje = 'Probando conexión...';
            try {
                const respuesta = await consultar_health();
                this.estado = 'ok';
                this.mensaje = `API Conectada (${respuesta.status || 'OK'})`;
            } catch (error) {
                this.estado = 'error';
                this.mensaje = error.mensaje || 'Error de conexión';
            }
        },
        abrirModalEdicion() {
            this.formEdit.nombre = this.usuarioActual.nombre || '';
            this.formEdit.apellido = this.usuarioActual.apellido || '';
            this.formEdit.email = this.usuarioActual.email || '';
            this.modalEdicionAbierto = true;
        },
        cerrarModalEdicion() {
            this.modalEdicionAbierto = false;
        },
        async guardarDatosEmail() {
            this.cargandoEmail = true;
            try {
                const idUsuario = this.usuarioActual.id || this.usuarioActual.Id;
                
                // Buscamos todas las posibles variantes de la propiedad de persona
                const personaIdAEnviar = this.usuarioActual.personaId || 
                                         this.usuarioActual.PersonaId || 
                                         this.usuarioActual.idPersona || 
                                         this.usuarioActual.IdPersona;

                // Buscamos todas las posibles variantes del rol
                const rolAEnviar = this.usuarioActual.rolId || 
                                   this.usuarioActual.RolId || 
                                   this.usuarioActual.rol || 
                                   this.usuarioActual.Rol || 'ADMIN';

                await actualizar_usuario(idUsuario, {
                    personaId: personaIdAEnviar,
                    email: this.formEdit.email,
                    rol: rolAEnviar
                });

                // Actualizamos el store localmente
                if (this.sesionStore.usuario) {
                    this.sesionStore.usuario.email = this.formEdit.email;
                }

                this.mostrarToast('¡Correo electrónico actualizado correctamente!', 'success');
                this.modalEdicionAbierto = false;
            } catch (error) {
                this.mostrarToast(error.mensaje || 'Error al actualizar el correo electrónico.', 'danger');
            } finally {
                this.cargandoEmail = false;
            }
        },
        async cambiarPassword() {
            if (this.passForm.nueva !== this.passForm.confirmar) {
                this.mostrarToast('Las contraseñas nuevas no coinciden.', 'warning');
                return;
            }

            this.cargandoPassword = true;
            try {
                await actualizar_password({
                    passwordActual: this.passForm.actual,
                    passwordNueva: this.passForm.nueva
                });

                this.mostrarToast('¡Contraseña actualizada con éxito!', 'success');
                this.passForm.actual = '';
                this.passForm.nueva = '';
                this.passForm.confirmar = '';
            } catch (error) {
                this.mostrarToast(error.mensaje || 'Contraseña actual incorrecta o error en el servidor.', 'danger');
            } finally {
                this.cargandoPassword = false;
            }
        },
        alertaFotoProximamente() {
            this.mostrarToast('La funcionalidad para cambiar la foto estará disponible próximamente.', 'medium');
        }
    }
};
</script>

<style scoped>
.perfil-container {
    max-width: 650px;
    margin: 0 auto;
}

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
    width: 60px;
    height: 60px;
    background-color: var(--ion-color-primary-tint, #1a59b3);
    color: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.5rem;
    font-weight: bold;
    border-radius: 50%;
}

.avatar-iniciales {
    letter-spacing: 1px;
}

.rol-badge-text {
    font-size: 0.85rem;
    margin-top: 2px;
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

.header-seccion, .header-diagnostico {
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

.input-item {
    background: var(--ion-background-color);
    border: 1px solid rgba(0, 0, 0, 0.1);
    border-radius: 10px;
    margin-bottom: 12px;
    --padding-start: 12px;
    --padding-end: 12px;
}

body.dark .input-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
}

.btn-accion {
    --border-radius: 10px;
    font-weight: 600;
    height: 44px;
}

/* Estilos refinados del modal */
.modal-body-container {
    max-width: 500px;
    margin: 0 auto;
    padding: 10px 4px;
}

.profile-photo-section {
    display: flex;
    align-items: center;
    gap: 20px;
    padding: 16px;
    background: rgba(var(--ion-color-primary-rgb), 0.04);
    border-radius: 14px;
    border: 1px dashed rgba(var(--ion-color-primary-rgb), 0.2);
}

.modal-avatar-preview {
    width: 72px;
    height: 72px;
    background-color: var(--ion-color-primary);
    color: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.8rem;
    font-weight: bold;
    border-radius: 50%;
    flex-shrink: 0;
}

.photo-actions h3 {
    font-size: 1rem;
    font-weight: 700;
    margin: 0 0 2px 0;
    color: var(--ion-text-color);
}

.photo-actions p {
    font-size: 0.8rem;
    color: var(--ion-color-medium);
    margin: 0;
}

.divider-custom {
    margin: 20px 0;
}

.seccion-bloqueada-info {
    margin-bottom: 16px;
}

.info-label-bloqueada {
    font-size: 0.78rem;
    font-weight: 600;
    color: var(--ion-color-medium);
    margin-bottom: 8px;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.input-item-bloqueado {
    background: var(--ion-color-step-100, #eaeaea);
    border-radius: 10px;
    margin-bottom: 10px;
    opacity: 0.8;
    --padding-start: 12px;
    --padding-end: 12px;
}

body.dark .input-item-bloqueado {
    background: var(--ion-color-step-850, #1f293d);
}

.input-hint {
    font-size: 0.75rem;
    color: var(--ion-color-medium);
    margin: -6px 0 16px 4px;
}
</style>