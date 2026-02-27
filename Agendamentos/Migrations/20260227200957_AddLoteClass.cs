using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agendamentos.Migrations
{
    /// <inheritdoc />
    public partial class AddLoteClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LoteId",
                table: "Horarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoteId",
                table: "Agendamentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_LoteId",
                table: "Horarios",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_LoteId",
                table: "Agendamentos",
                column: "LoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Lotes_LoteId",
                table: "Agendamentos",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Horarios_Lotes_LoteId",
                table: "Horarios",
                column: "LoteId",
                principalTable: "Lotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Lotes_LoteId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Horarios_Lotes_LoteId",
                table: "Horarios");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropIndex(
                name: "IX_Horarios_LoteId",
                table: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_LoteId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "LoteId",
                table: "Horarios");

            migrationBuilder.DropColumn(
                name: "LoteId",
                table: "Agendamentos");
        }
    }
}
