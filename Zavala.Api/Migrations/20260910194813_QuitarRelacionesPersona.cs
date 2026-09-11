using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavala.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuitarRelacionesPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alumnos_personas_PersonaId1",
                table: "alumnos");

            migrationBuilder.DropForeignKey(
                name: "FK_docentes_personas_PersonaId1",
                table: "docentes");

            migrationBuilder.DropForeignKey(
                name: "FK_preceptores_personas_PersonaId1",
                table: "preceptores");

            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_personas_PersonaId1",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_PersonaId1",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_preceptores_PersonaId1",
                table: "preceptores");

            migrationBuilder.DropIndex(
                name: "IX_docentes_PersonaId1",
                table: "docentes");

            migrationBuilder.DropIndex(
                name: "IX_alumnos_PersonaId1",
                table: "alumnos");

            migrationBuilder.DropColumn(
                name: "PersonaId1",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "PersonaId1",
                table: "preceptores");

            migrationBuilder.DropColumn(
                name: "PersonaId1",
                table: "docentes");

            migrationBuilder.DropColumn(
                name: "PersonaId1",
                table: "alumnos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonaId1",
                table: "usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonaId1",
                table: "preceptores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonaId1",
                table: "docentes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonaId1",
                table: "alumnos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_PersonaId1",
                table: "usuarios",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preceptores_PersonaId1",
                table: "preceptores",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_docentes_PersonaId1",
                table: "docentes",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alumnos_PersonaId1",
                table: "alumnos",
                column: "PersonaId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_alumnos_personas_PersonaId1",
                table: "alumnos",
                column: "PersonaId1",
                principalTable: "personas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_docentes_personas_PersonaId1",
                table: "docentes",
                column: "PersonaId1",
                principalTable: "personas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_preceptores_personas_PersonaId1",
                table: "preceptores",
                column: "PersonaId1",
                principalTable: "personas",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_personas_PersonaId1",
                table: "usuarios",
                column: "PersonaId1",
                principalTable: "personas",
                principalColumn: "id");
        }
    }
}
