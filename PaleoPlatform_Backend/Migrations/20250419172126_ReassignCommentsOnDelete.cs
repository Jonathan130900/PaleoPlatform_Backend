using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ReassignCommentsOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti");

            migrationBuilder.AlterColumn<int>(
                name: "ArticoloId",
                table: "Commenti",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti",
                column: "ArticoloId",
                principalTable: "Articoli",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti");

            migrationBuilder.AlterColumn<int>(
                name: "ArticoloId",
                table: "Commenti",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti",
                column: "ArticoloId",
                principalTable: "Articoli",
                principalColumn: "Id");
        }
    }
}
