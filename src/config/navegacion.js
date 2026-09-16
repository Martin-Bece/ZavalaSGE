import { 
  homeOutline, 
  peopleOutline, 
  schoolOutline, 
  documentTextOutline, 
  calendarOutline, 
  peopleCircleOutline, 
  clipboardOutline, 
  bookOutline, 
  personOutline 
} from 'ionicons/icons';

// Grupos para organizar el menú lateral (con su propio campo orden)
export const grupos_menu = [
  { id: 'principal', titulo: 'Principal', orden: 1 },
  { id: 'gestion', titulo: 'Gestión Institucional', orden: 2 },
  { id: 'operativo', titulo: 'Operaciones', orden: 3 },
  { id: 'cuenta', titulo: 'Mi Cuenta', orden: 4 }
];

// Registro unificado de todas las pantallas
export const navegacion = [
  {
    id: 'inicio',
    titulo: 'Inicio',
    ruta: '/app/inicio',
    icono: homeOutline,
    grupo_menu: 'principal',
    orden: 1,
    roles: ['ADMIN', 'PRECEPTOR', 'DOCENTE'],
    componente: () => import('@/views/inicio_page.vue')
  },
  {
    id: 'personal',
    titulo: 'Personal',
    ruta: '/app/personal',
    icono: peopleOutline,
    grupo_menu: 'gestion',
    orden: 2,
    roles: ['ADMIN'],
    componente: () => import('@/views/personal_page.vue')
  },
  {
    id: 'academico',
    titulo: 'Académico',
    ruta: '/app/academico',
    icono: schoolOutline,
    grupo_menu: 'gestion',
    orden: 3,
    roles: ['ADMIN'],
    componente: () => import('@/views/academico_page.vue')
  },
  {
    id: 'reportes',
    titulo: 'Reportes',
    ruta: '/app/reportes',
    icono: documentTextOutline,
    grupo_menu: 'gestion',
    orden: 4,
    roles: ['ADMIN'],
    componente: () => import('@/views/reportes_page.vue')
  },
  {
    id: 'asistencia',
    titulo: 'Asistencia',
    ruta: '/app/asistencia',
    icono: calendarOutline,
    grupo_menu: 'operativo',
    orden: 5,
    roles: ['PRECEPTOR'],
    componente: () => import('@/views/asistencia_page.vue')
  },
  {
    id: 'comisiones',
    titulo: 'Comisiones',
    ruta: '/app/comisiones',
    icono: peopleCircleOutline,
    grupo_menu: 'operativo',
    orden: 6,
    roles: ['PRECEPTOR'],
    componente: () => import('@/views/comisiones_page.vue')
  },
  {
    id: 'calificaciones',
    titulo: 'Calificaciones',
    ruta: '/app/calificaciones',
    icono: clipboardOutline,
    grupo_menu: 'operativo',
    orden: 7,
    roles: ['PRECEPTOR', 'DOCENTE'],
    componente: () => import('@/views/calificaciones_page.vue')
  },
  {
    id: 'materias',
    titulo: 'Mis Materias',
    ruta: '/app/materias',
    icono: bookOutline,
    grupo_menu: 'operativo',
    orden: 8,
    roles: ['DOCENTE'],
    componente: () => import('@/views/materias_page.vue')
  },
  {
    id: 'perfil',
    titulo: 'Mi Perfil',
    ruta: '/app/perfil',
    icono: personOutline,
    grupo_menu: 'cuenta',
    orden: 9,
    roles: ['ADMIN', 'PRECEPTOR', 'DOCENTE'],
    componente: () => import('@/views/perfil_page.vue')
  }
];

export function obtener_grupos_menu(rol_activo = null){
  // Normalizar el rol activo a mayúsculas si existe
  const rolNormalizado = typeof rol_activo === 'string' ? rol_activo.toUpperCase() : rol_activo;

  const items = [...navegacion]
    .filter(item => !item.roles || item.roles.some(r => r.toUpperCase() === rolNormalizado))
    .sort((item_a, item_b) => item_a.orden - item_b.orden);

  let grupos = grupos_menu.map(grupo => ({
    ...grupo,
    items: items.filter(item => item.grupo_menu === grupo.id)
  }));

  grupos = grupos.filter(grupo => grupo.items.length > 0);
  grupos.sort((grupo_a, grupo_b) => grupo_a.orden - grupo_b.orden);
  return grupos;
}

export function obtener_tabs(){
	// La misma lista completa, ordenada por el campo orden.
	return [...navegacion].sort((item_a, item_b) => item_a.orden - item_b.orden);
}