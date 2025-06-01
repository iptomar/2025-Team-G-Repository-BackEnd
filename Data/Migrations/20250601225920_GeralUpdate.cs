using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndHorario.Data.Migrations
{
    /// <inheritdoc />
    public partial class GeralUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnoLetivo",
                table: "Horarios");

            migrationBuilder.DropColumn(
                name: "Dia",
                table: "Blocos");

            migrationBuilder.DropColumn(
                name: "HoraInicio",
                table: "Blocos");

            migrationBuilder.RenameColumn(
                name: "Semestre",
                table: "Horarios",
                newName: "Ano");

            migrationBuilder.AddColumn<int>(
                name: "Ano",
                table: "UnidadesCurriculares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DocentePL",
                table: "UnidadesCurriculares",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocenteTP",
                table: "UnidadesCurriculares",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorasPL",
                table: "UnidadesCurriculares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HorasTP",
                table: "UnidadesCurriculares",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalaPLId",
                table: "UnidadesCurriculares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalaTPId",
                table: "UnidadesCurriculares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RepetirSemanas",
                table: "Blocos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesCurriculares_SalaPLId",
                table: "UnidadesCurriculares",
                column: "SalaPLId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesCurriculares_SalaTPId",
                table: "UnidadesCurriculares",
                column: "SalaTPId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesCurriculares_Salas_SalaPLId",
                table: "UnidadesCurriculares",
                column: "SalaPLId",
                principalTable: "Salas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesCurriculares_Salas_SalaTPId",
                table: "UnidadesCurriculares",
                column: "SalaTPId",
                principalTable: "Salas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnidadesCurriculares_Salas_SalaPLId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropForeignKey(
                name: "FK_UnidadesCurriculares_Salas_SalaTPId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesCurriculares_SalaPLId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesCurriculares_SalaTPId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "Ano",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "DocentePL",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "DocenteTP",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "HorasPL",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "HorasTP",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "SalaPLId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "SalaTPId",
                table: "UnidadesCurriculares");

            migrationBuilder.DropColumn(
                name: "RepetirSemanas",
                table: "Blocos");

            migrationBuilder.RenameColumn(
                name: "Ano",
                table: "Horarios",
                newName: "Semestre");

            migrationBuilder.AddColumn<string>(
                name: "AnoLetivo",
                table: "Horarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dia",
                table: "Blocos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraInicio",
                table: "Blocos",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
