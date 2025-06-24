using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndHorario.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAssociacoesUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CursoId",
                table: "Utilizadores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscolaId",
                table: "Utilizadores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_CursoId",
                table: "Utilizadores",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_EscolaId",
                table: "Utilizadores",
                column: "EscolaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Cursos_CursoId",
                table: "Utilizadores",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Escolas_EscolaId",
                table: "Utilizadores",
                column: "EscolaId",
                principalTable: "Escolas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Cursos_CursoId",
                table: "Utilizadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Escolas_EscolaId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_CursoId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_EscolaId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "CursoId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "EscolaId",
                table: "Utilizadores");
        }
    }
}
