using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavala.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurarCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cursos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ciclo_lectivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cursos", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "materias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materias", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "personas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    dni = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    apellido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personas", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "alumnos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    legajo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    persona_id = table.Column<int>(type: "int", nullable: false),
                    PersonaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alumnos", x => x.id);
                    table.ForeignKey(
                        name: "FK_alumnos_personas_PersonaId1",
                        column: x => x.PersonaId1,
                        principalTable: "personas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_alumnos_personas_persona_id",
                        column: x => x.persona_id,
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "docentes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    legajo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    titulo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    persona_id = table.Column<int>(type: "int", nullable: false),
                    PersonaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_docentes", x => x.id);
                    table.ForeignKey(
                        name: "FK_docentes_personas_PersonaId1",
                        column: x => x.PersonaId1,
                        principalTable: "personas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_docentes_personas_persona_id",
                        column: x => x.persona_id,
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "preceptores",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    legajo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    persona_id = table.Column<int>(type: "int", nullable: false),
                    PersonaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preceptores", x => x.id);
                    table.ForeignKey(
                        name: "FK_preceptores_personas_PersonaId1",
                        column: x => x.PersonaId1,
                        principalTable: "personas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_preceptores_personas_persona_id",
                        column: x => x.persona_id,
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rol = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    persona_id = table.Column<int>(type: "int", nullable: false),
                    PersonaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarios_personas_PersonaId1",
                        column: x => x.PersonaId1,
                        principalTable: "personas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_usuarios_personas_persona_id",
                        column: x => x.persona_id,
                        principalTable: "personas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comisiones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    curso_id = table.Column<int>(type: "int", nullable: false),
                    preceptor_id = table.Column<int>(type: "int", nullable: false),
                    CursoId1 = table.Column<int>(type: "int", nullable: true),
                    PreceptoreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comisiones", x => x.id);
                    table.ForeignKey(
                        name: "FK_comisiones_cursos_CursoId1",
                        column: x => x.CursoId1,
                        principalTable: "cursos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comisiones_cursos_curso_id",
                        column: x => x.curso_id,
                        principalTable: "cursos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comisiones_preceptores_PreceptoreId",
                        column: x => x.PreceptoreId,
                        principalTable: "preceptores",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comisiones_preceptores_preceptor_id",
                        column: x => x.preceptor_id,
                        principalTable: "preceptores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "asignaciones_docente",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    docente_id = table.Column<int>(type: "int", nullable: false),
                    materia_id = table.Column<int>(type: "int", nullable: false),
                    comision_id = table.Column<int>(type: "int", nullable: false),
                    ComisioneId = table.Column<int>(type: "int", nullable: true),
                    MateriaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asignaciones_docente", x => x.id);
                    table.ForeignKey(
                        name: "FK_asignaciones_docente_comisiones_ComisioneId",
                        column: x => x.ComisioneId,
                        principalTable: "comisiones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_asignaciones_docente_comisiones_comision_id",
                        column: x => x.comision_id,
                        principalTable: "comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asignaciones_docente_docentes_docente_id",
                        column: x => x.docente_id,
                        principalTable: "docentes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asignaciones_docente_materias_MateriaId1",
                        column: x => x.MateriaId1,
                        principalTable: "materias",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_asignaciones_docente_materias_materia_id",
                        column: x => x.materia_id,
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "asistencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    alumno_id = table.Column<int>(type: "int", nullable: false),
                    comision_id = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlumnoId1 = table.Column<int>(type: "int", nullable: true),
                    ComisioneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asistencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_asistencias_alumnos_AlumnoId1",
                        column: x => x.AlumnoId1,
                        principalTable: "alumnos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_asistencias_alumnos_alumno_id",
                        column: x => x.alumno_id,
                        principalTable: "alumnos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asistencias_comisiones_ComisioneId",
                        column: x => x.ComisioneId,
                        principalTable: "comisiones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_asistencias_comisiones_comision_id",
                        column: x => x.comision_id,
                        principalTable: "comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "calificaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    alumno_id = table.Column<int>(type: "int", nullable: false),
                    materia_id = table.Column<int>(type: "int", nullable: false),
                    comision_id = table.Column<int>(type: "int", nullable: false),
                    nota = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    instancia = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha = table.Column<DateOnly>(type: "date", nullable: true),
                    AlumnoId1 = table.Column<int>(type: "int", nullable: true),
                    ComisioneId = table.Column<int>(type: "int", nullable: true),
                    MateriaId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calificaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_calificaciones_alumnos_AlumnoId1",
                        column: x => x.AlumnoId1,
                        principalTable: "alumnos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_calificaciones_alumnos_alumno_id",
                        column: x => x.alumno_id,
                        principalTable: "alumnos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_calificaciones_comisiones_ComisioneId",
                        column: x => x.ComisioneId,
                        principalTable: "comisiones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_calificaciones_comisiones_comision_id",
                        column: x => x.comision_id,
                        principalTable: "comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_calificaciones_materias_MateriaId1",
                        column: x => x.MateriaId1,
                        principalTable: "materias",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_calificaciones_materias_materia_id",
                        column: x => x.materia_id,
                        principalTable: "materias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inscripciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    alumno_id = table.Column<int>(type: "int", nullable: false),
                    comision_id = table.Column<int>(type: "int", nullable: false),
                    fecha_inscripcion = table.Column<DateOnly>(type: "date", nullable: true),
                    AlumnoId1 = table.Column<int>(type: "int", nullable: true),
                    ComisioneId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscripciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_inscripciones_alumnos_AlumnoId1",
                        column: x => x.AlumnoId1,
                        principalTable: "alumnos",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inscripciones_alumnos_alumno_id",
                        column: x => x.alumno_id,
                        principalTable: "alumnos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inscripciones_comisiones_ComisioneId",
                        column: x => x.ComisioneId,
                        principalTable: "comisiones",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_inscripciones_comisiones_comision_id",
                        column: x => x.comision_id,
                        principalTable: "comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_alumnos_legajo",
                table: "alumnos",
                column: "legajo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alumnos_persona_id",
                table: "alumnos",
                column: "persona_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alumnos_PersonaId1",
                table: "alumnos",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_docente_comision_id",
                table: "asignaciones_docente",
                column: "comision_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_docente_ComisioneId",
                table: "asignaciones_docente",
                column: "ComisioneId");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_docente_docente_id",
                table: "asignaciones_docente",
                column: "docente_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_docente_materia_id",
                table: "asignaciones_docente",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_docente_MateriaId1",
                table: "asignaciones_docente",
                column: "MateriaId1");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_alumno_id",
                table: "asistencias",
                column: "alumno_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_AlumnoId1",
                table: "asistencias",
                column: "AlumnoId1");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_comision_id",
                table: "asistencias",
                column: "comision_id");

            migrationBuilder.CreateIndex(
                name: "IX_asistencias_ComisioneId",
                table: "asistencias",
                column: "ComisioneId");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_alumno_id",
                table: "calificaciones",
                column: "alumno_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_AlumnoId1",
                table: "calificaciones",
                column: "AlumnoId1");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_comision_id",
                table: "calificaciones",
                column: "comision_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_ComisioneId",
                table: "calificaciones",
                column: "ComisioneId");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_materia_id",
                table: "calificaciones",
                column: "materia_id");

            migrationBuilder.CreateIndex(
                name: "IX_calificaciones_MateriaId1",
                table: "calificaciones",
                column: "MateriaId1");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_curso_id",
                table: "comisiones",
                column: "curso_id");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_CursoId1",
                table: "comisiones",
                column: "CursoId1");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_preceptor_id",
                table: "comisiones",
                column: "preceptor_id");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_PreceptoreId",
                table: "comisiones",
                column: "PreceptoreId");

            migrationBuilder.CreateIndex(
                name: "IX_docentes_legajo",
                table: "docentes",
                column: "legajo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_docentes_persona_id",
                table: "docentes",
                column: "persona_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_docentes_PersonaId1",
                table: "docentes",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inscripciones_alumno_id",
                table: "inscripciones",
                column: "alumno_id");

            migrationBuilder.CreateIndex(
                name: "IX_inscripciones_AlumnoId1",
                table: "inscripciones",
                column: "AlumnoId1");

            migrationBuilder.CreateIndex(
                name: "IX_inscripciones_comision_id",
                table: "inscripciones",
                column: "comision_id");

            migrationBuilder.CreateIndex(
                name: "IX_inscripciones_ComisioneId",
                table: "inscripciones",
                column: "ComisioneId");

            migrationBuilder.CreateIndex(
                name: "IX_materias_codigo",
                table: "materias",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_personas_dni",
                table: "personas",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preceptores_legajo",
                table: "preceptores",
                column: "legajo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preceptores_persona_id",
                table: "preceptores",
                column: "persona_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preceptores_PersonaId1",
                table: "preceptores",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_persona_id",
                table: "usuarios",
                column: "persona_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_PersonaId1",
                table: "usuarios",
                column: "PersonaId1",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asignaciones_docente");

            migrationBuilder.DropTable(
                name: "asistencias");

            migrationBuilder.DropTable(
                name: "calificaciones");

            migrationBuilder.DropTable(
                name: "inscripciones");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "docentes");

            migrationBuilder.DropTable(
                name: "materias");

            migrationBuilder.DropTable(
                name: "alumnos");

            migrationBuilder.DropTable(
                name: "comisiones");

            migrationBuilder.DropTable(
                name: "cursos");

            migrationBuilder.DropTable(
                name: "preceptores");

            migrationBuilder.DropTable(
                name: "personas");
        }
    }
}
