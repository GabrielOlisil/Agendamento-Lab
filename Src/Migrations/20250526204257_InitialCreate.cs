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
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false)
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
                name: "Professores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Matricula = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AmbienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    HorarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Agendamentos_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    PassWordHash = table.Column<string>(type: "text", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Horarios",
                columns: new[] { "Id", "Inicio", "Rank", "Turno" },
                values: new object[,]
                {
                    { new Guid("10101010-1010-1010-1010-101010101010"), new TimeSpan(0, 11, 0, 0, 0), 10, 2 },
                    { new Guid("11011011-1101-1101-1101-110110110110"), new TimeSpan(0, 11, 0, 0, 0), 11, 2 },
                    { new Guid("11111111-1111-1111-1111-111111111111"), new TimeSpan(0, 8, 0, 0, 0), 1, 0 },
                    { new Guid("12121212-1212-1212-1212-121212121212"), new TimeSpan(0, 11, 0, 0, 0), 12, 2 },
                    { new Guid("13131313-1313-1313-1313-131313131313"), new TimeSpan(0, 11, 0, 0, 0), 13, 2 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new TimeSpan(0, 9, 0, 0, 0), 2, 0 },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new TimeSpan(0, 10, 0, 0, 0), 3, 0 },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new TimeSpan(0, 11, 0, 0, 0), 4, 0 },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new TimeSpan(0, 11, 0, 0, 0), 5, 1 },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new TimeSpan(0, 11, 0, 0, 0), 6, 1 },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new TimeSpan(0, 11, 0, 0, 0), 7, 1 },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new TimeSpan(0, 11, 0, 0, 0), 8, 1 },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new TimeSpan(0, 11, 0, 0, 0), 9, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_AmbienteId",
                table: "Agendamentos",
                column: "AmbienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_HorarioId",
                table: "Agendamentos",
                column: "HorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ProfessorId",
                table: "Agendamentos",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ambientes_Slug",
                table: "Ambientes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProfessorId",
                table: "Users",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Ambientes");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "Professores");
        }
    }
}
