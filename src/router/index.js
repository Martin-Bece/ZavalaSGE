import { createRouter, createWebHistory } from "@ionic/vue-router";
import { navegacion } from "@/config/navegacion";
import main_layout from "@/layouts/main_layout.vue";
import { use_sesion_store } from "@/stores/sesion_store";

// 1. Mapear las rutas pasando la metadata de los roles permitidos
const rutas_app = navegacion.map((item) => ({
  path: item.ruta.replace("/app/", ""),
  name: item.id,
  component: item.componente,
  meta: { roles: item.roles },
}));

const routes = [
  {
    path: "/",
    redirect: "/login",
  },
  {
    path: "/login",
    name: "login",
    meta: { publica: true },
    component: () => import("@/views/login_page.vue"),
  },
  {
    path: "/app",
    component: main_layout,
    children: [
      {
        path: "",
        redirect: "/app/inicio",
      },
      {
        path: "alumno/:id",
        name: "alumno_detalle",
        component: () => import("@/views/alumno_detalle_page.vue"),
        meta: { roles: ["ADMIN", "PRECEPTOR", "DOCENTE"] },
      },
      {
        path: "notificaciones",
        name: "notificaciones",
        component: () => import("@/views/notificaciones_page.vue"),
        meta: { roles: ["ADMIN", "PRECEPTOR", "DOCENTE"] },
      },
      ...rutas_app,
    ],
  },
  {
    // Si entra a cualquier ruta desconocida, mandarlo a login por seguridad
    path: "/:pathMatch(.*)*",
    redirect: "/login",
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

// 2. Guard de navegación asíncrono
router.beforeEach(async (to) => {
  const sesion = use_sesion_store();

  // 1. ESPERAR a que la sesión intente restaurarse antes de tomar decisiones
  if (sesion.restaurando) {
    await sesion.preparar();
    await sesion.iniciar();
  }

  console.log("===> USUARIO CARGADO:", sesion.usuario);
  console.log("===> ROL ACTIVO EN STORE:", sesion.rol_activo);

  // 2. Si la ruta es pública (Login)
  if (to.meta.publica) {
    // Si ya hay alguien logueado, recién ahí lo mandamos adentro
    if (sesion.autenticado) {
      return { path: "/app/inicio", replace: true };
    }
    return true;
  }

  // 3. Si NO está autenticado, va SIEMPRE al login (sin importar a qué URL quiso entrar)
  if (!sesion.autenticado) {
    return { path: "/login", replace: true };
  }

  // 4. Si ESTÁ AUTENTICADO y refrescó la página en una URL específica (ej: /app/alumnos),
  // el guard pasa a evaluar los permisos para esa ruta y se queda ahí.
  const roles_permitidos = to.meta.roles;
  if (!roles_permitidos || roles_permitidos.length === 0) return true;

  const tiene_permiso = roles_permitidos.includes(sesion.rol_activo);

  if (tiene_permiso) return true;

  // Evitar bucle en inicio si no tiene rol para la vista solicitada
  if (to.path === "/app/inicio") return true;

  return { path: "/app/inicio", replace: true };
});

export default router;
