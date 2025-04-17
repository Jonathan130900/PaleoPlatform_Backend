using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentiNavigationToArticolo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticoloId",
                table: "Commenti",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commenti_ArticoloId",
                table: "Commenti",
                column: "ArticoloId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti",
                column: "ArticoloId",
                principalTable: "Articoli",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti");

            migrationBuilder.DropIndex(
                name: "IX_Commenti_ArticoloId",
                table: "Commenti");

            migrationBuilder.DropColumn(
                name: "ArticoloId",
                table: "Commenti");
        }
    }
}
