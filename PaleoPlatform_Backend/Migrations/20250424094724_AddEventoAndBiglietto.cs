using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventoAndBiglietto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Luogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PostiDisponibili = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Biglietti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSold = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biglietti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Biglietti_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Biglietti_Eventi_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Biglietti_EventoId",
                table: "Biglietti",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Biglietti_UtenteId",
                table: "Biglietti",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Biglietti");

            migrationBuilder.DropTable(
                name: "Eventi");
        }
    }
}
