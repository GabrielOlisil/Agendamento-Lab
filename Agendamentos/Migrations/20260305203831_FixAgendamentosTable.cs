using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agendamentos.Migrations
{
    /// <inheritdoc />
    public partial class FixAgendamentosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_Lotes_LoteId",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Horarios_LoteId",
                table: "Horarios");

            migrationBuilder.DropColumn(
                name: "LoteId",
                table: "Horarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoteId",
                table: "Horarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_LoteId",
                table: "Horarios",
                column: "LoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_Lotes_LoteId",
                table: "Horarios",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id");
        }
    }
}
