using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixEventoPartecipazioneModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoPartecipazioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    DataPartecipazione = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPartecipazioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoPartecipazioni_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPartecipazioni_Eventi_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoPartecipazioni_EventoId",
                table: "EventoPartecipazioni",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPartecipazioni_UtenteId_EventoId",
                table: "EventoPartecipazioni",
                columns: new[] { "UtenteId", "EventoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoPartecipazioni");
        }
    }
}
