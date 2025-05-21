using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Agendamentos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ambientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ambientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Inicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Turno = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AmbienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    HorarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Ambientes_AmbienteId",
                        column: x => x.AmbienteId,
                        principalTable: "Ambientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Horarios_HorarioId",
                        column: x => x.HorarioId,
                        principalTable: "Horarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Horarios",
                columns: new[] { "Id", "Inicio", "Rank", "Turno" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new TimeSpan(0, 8, 0, 0, 0), 1, 0 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new TimeSpan(0, 9, 0, 0, 0), 2, 0 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new TimeSpan(0, 10, 0, 0, 0), 3, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new TimeSpan(0, 11, 0, 0, 0), 4, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_AmbienteId",
                table: "Agendamentos",
                column: "AmbienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos",
                column: "HorarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Ambientes");

            migrationBuilder.DropTable(
                name: "Horarios");
        }
    }
}
