using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SlightChangesWithReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentoId",
                table: "Commenti",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commenti_CommentoId",
                table: "Commenti",
                column: "CommentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Commenti_CommentoId",
                table: "Commenti",
                column: "CommentoId",
                principalTable: "Commenti",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Commenti_CommentoId",
                table: "Commenti");

            migrationBuilder.DropIndex(
                name: "IX_Commenti_CommentoId",
                table: "Commenti");

            migrationBuilder.DropColumn(
                name: "CommentoId",
                table: "Commenti");
        }
    }
}
