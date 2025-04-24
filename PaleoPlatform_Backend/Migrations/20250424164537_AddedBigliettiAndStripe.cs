using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedBigliettiAndStripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSold",
                table: "Biglietti",
                newName: "Pagato");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAcquisto",
                table: "Biglietti",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Prezzo",
                table: "Biglietti",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAcquisto",
                table: "Biglietti");

            migrationBuilder.DropColumn(
                name: "Prezzo",
                table: "Biglietti");

            migrationBuilder.RenameColumn(
                name: "Pagato",
                table: "Biglietti",
                newName: "IsSold");
        }
    }
}
