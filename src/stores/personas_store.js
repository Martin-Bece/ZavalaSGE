import { defineStore } from "pinia";
import {
  obtener_docentes,
  crear_docente,
  actualizar_docente,
  eliminar_docente,
} from "@/services/docentes_service";
import {
  obtener_preceptores,
  crear_preceptor,
  actualizar_preceptor,
  eliminar_preceptor,
} from "@/services/preceptores_service";
import {
  obtener_alumnos,
  crear_alumno,
  actualizar_alumno,
  eliminar_alumno,
} from "@/services/alumnos_service";
import { crear_persona, actualizar_persona, eliminar_persona } from "@/services/personas_service";

export const use_personas_store = defineStore("personas", {
  state: () => ({
    personas: [],
    pagina: null,
    filtros: {
      busqueda: "",
      rol: null, // null | 'docente' | 'preceptor' 
    },
    cargando: false,
    guardando: false,
    error: null,
  }),
  getters: {
    hay_personas(state) {
      return state.personas.length > 0;
    },
    hay_mas(state) {
      if (!state.pagina) return false;
      return state.pagina.hayMas !== undefined
        ? Boolean(state.pagina.hayMas)
        : Boolean(state.pagina.tienePaginaSiguiente);
    },
    total(state) {
      return state.pagina?.total || state.pagina?.totalItems || 0;
    },
  },
  actions: {
    cargar: async function (filtros = null) {
      var vm = this;
      vm.cargando = true;
      vm.error = null;
      if (filtros) vm.filtros = { ...vm.filtros, ...filtros };

      try {
        const res = await vm._obtener_segun_filtro(1);
        vm.personas = res.personas;
        vm.pagina = res.pagina;
        return vm.personas;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar las personas.";
        vm.personas = [];
        vm.pagina = null;
        return [];
      } finally {
        vm.cargando = false;
      }
    },

    cargar_mas: async function () {
      var vm = this;
      if (vm.cargando || !vm.hay_mas) return vm.personas;
      vm.cargando = true;
      try {
        const paginaActual = vm.pagina?.pagina || 1;
        const paginaSiguiente = paginaActual + 1;

        const res = await vm._obtener_segun_filtro(paginaSiguiente);

        const conocidos = vm.personas.map((p) => `${p.rol}-${p.id}`);
        const nuevas = res.personas.filter(
          (p) => !conocidos.includes(`${p.rol}-${p.id}`),
        );

        vm.personas = [...vm.personas, ...nuevas];
        vm.pagina = res.pagina;
        return vm.personas;
      } catch (error) {
        vm.error = error.mensaje || "No se pudieron cargar más personas.";
        vm.personas = vm.personas;
        return vm.personas;
      } finally {
        vm.cargando = false;
      }
    },

    guardar: async function (id, datos, rol) {
      var vm = this;
      vm.guardando = true;
      vm.error = null;

      const rolNormalizado = rol?.toLowerCase();

      try {
        // 1. PRIMERO GUARDAMOS O ACTUALIZAMOS LA PERSONA BASE
        const datosPersona = {
          dni: datos.dni,
          nombre: datos.nombre,
          apellido: datos.apellido,
          telefono: datos.telefono,
          direccion: datos.direccion,
          fechaNacimiento: datos.fecha_nacimiento || datos.FechaNacimiento,
        };

        // Buscamos el ID de la persona de cualquier variante posible (camelCase, PascalCase, snake_case)
        let personaIdFinal =
          datos.personaId || datos.PersonaId || datos.persona_id;

        if (id && personaIdFinal) {
          // Si estamos editando y tenemos el ID de la persona, actualizamos sus datos base
          await actualizar_persona(personaIdFinal, datosPersona);
        } else if (id && !personaIdFinal) {
          // Por seguridad: si hay 'id' (de docente/alumno) pero no vino el personaId,
          // intentamos asumirlo o frenar antes de crear uno duplicado.
          // (Idealmente tu componente de Vue siempre debe mandar el personaId al editar).
          console.warn(
            "Se detectó un ID de rol pero falta el personaId. Usando id de los datos si aplica.",
          );
          personaIdFinal = datos.id || datos.idPersona;
          if (personaIdFinal) {
            await actualizar_persona(personaIdFinal, datosPersona);
          } else {
            throw new Error(
              "No se encontró el ID de la persona para actualizar.",
            );
          }
        } else {
          // Si es nuevo (no hay id de rol), creamos la persona primero para obtener su ID
          const resPersona = await crear_persona(datosPersona);
          // Capturamos el ID según cómo responda tu API (resPersona.id, resPersona.persona.id, etc.)
          personaIdFinal =
            resPersona.id || resPersona.persona?.id || resPersona.personaId;
        }

        // 2. DESPUÉS GUARDAMOS O ACTUALIZAMOS EL ROL ESPECÍFICO (Docente, Preceptor o Alumno)
        const datosRol = {
          personaId: personaIdFinal,
          legajo: datos.legajo,
          titulo: datos.titulo, // Solo aplica si es docente
        };

        if (id) {
          // EDICIÓN DE ROL
          if (rolNormalizado === "docente") {
            await actualizar_docente(id, datosRol);
          } else if (rolNormalizado === "preceptor") {
            await actualizar_preceptor(id, datosRol);
          } else if (rolNormalizado === "alumno") {
            await actualizar_alumno(id, datosRol);
          }
        } else {
          // CREACIÓN DE ROL
          if (rolNormalizado === "docente") {
            await crear_docente(datosRol);
          } else if (rolNormalizado === "preceptor") {
            await crear_preceptor(datosRol);
          } else if (rolNormalizado === "alumno") {
            await crear_alumno(datosRol);
          }
        }

        await vm.cargar();
        return true;
      } catch (error) {
        vm.error =
          error.mensaje ||
          error.response?.data?.mensaje ||
          error.response?.data?.title ||
          "No se pudo guardar la persona.";
        console.error("Error al guardar:", error);
        return false;
      } finally {
        vm.guardando = false;
      }
    },

    eliminar: async function (id_o_persona) {
      var vm = this;
      vm.guardando = true;
      vm.error = null;

      console.log("Objeto o ID recibido en eliminar:", id_o_persona);

      const idPersona =
        typeof id_o_persona === "object"
          ? id_o_persona.personaId ||
            id_o_persona.PersonaId ||
            id_o_persona.id_persona ||
            id_o_persona.id
          : id_o_persona;

      console.log("ID final que se mandará a la API de personas:", idPersona);

      try {
        await eliminar_persona(idPersona);
        await vm.cargar();
        return true;
      } catch (error) {
        vm.error = error.mensaje || "No se pudo eliminar la persona.";
        return false;
      } finally {
        vm.guardando = false;
      }
    },

    // Helper privado normalizado
    _obtener_segun_filtro: async function (numPagina) {
      var vm = this;
      const params = {
        busqueda: vm.filtros.busqueda,
        pagina: numPagina,
        tamano: 10,
      };

      const rol = vm.filtros.rol?.toLowerCase();

      if (rol === "docente") {
        const res = await obtener_docentes(params);
        return {
          personas: (res.docentes || []).map((d) => ({
            ...d,
            rol: "docente",
            personaId: d.personaId || d.PersonaId, // <-- Forzamos a que siempre exista personaId
          })),
          pagina: res.pagina,
        };
      }

      if (rol === "preceptor") {
        const res = await obtener_preceptores(params);
        return {
          personas: (res.preceptores || []).map((p) => ({
            ...p,
            rol: "preceptor",
          })),
          pagina: res.pagina,
        };
      }

      // Si es 'todos' o null: traemos docentes, preceptores y alumnos en paralelo
      const [resDoc, resPrec] = await Promise.all([
        obtener_docentes(params),
        obtener_preceptores(params),
      ]);

      const docentes = (resDoc.docentes || []).map((d) => ({
        ...d,
        rol: "docente",
      }));
      const preceptores = (resPrec.preceptores || []).map((p) => ({
        ...p,
        rol: "preceptor",
      }));
      

      const combinados = [...docentes, ...preceptores].sort(
        (a, b) =>
          (a.personaNombre || a.nombre || a.apellido || "").localeCompare(
            b.personaNombre || b.nombre || b.apellido || "",
          ),
      );

      const total =
        (resDoc.pagina?.total || resDoc.pagina?.totalItems || 0) +
        (resPrec.pagina?.total || resPrec.pagina?.totalItems || 0);

      const hayMas =
        Boolean(resDoc.pagina?.hayMas || resDoc.pagina?.tienePaginaSiguiente) ||
        Boolean(resPrec.pagina?.hayMas || resPrec.pagina?.tienePaginaSiguiente);

      return {
        personas: combinados,
        pagina: {
          pagina: numPagina,
          total: total,
          hayMas: hayMas,
        },
      };
    },
  },
});
