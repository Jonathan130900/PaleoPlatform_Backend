using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToCarrello : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCreazione",
                table: "Carrelli",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPagamento",
                table: "Carrelli",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pagato",
                table: "Carrelli",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeCheckoutSessionId",
                table: "Carrelli",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripePaymentIntentId",
                table: "Biglietti",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCreazione",
                table: "Carrelli");

            migrationBuilder.DropColumn(
                name: "DataPagamento",
                table: "Carrelli");

            migrationBuilder.DropColumn(
                name: "Pagato",
                table: "Carrelli");

            migrationBuilder.DropColumn(
                name: "StripeCheckoutSessionId",
                table: "Carrelli");

            migrationBuilder.DropColumn(
                name: "StripePaymentIntentId",
                table: "Biglietti");
        }
    }
}
