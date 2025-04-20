using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class DeletedStupidUselessEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti");

            migrationBuilder.DropTable(
                name: "DiscussioneVoto");

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
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DiscussioneVoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscussioneId = table.Column<int>(type: "int", nullable: false),
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Valore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussioneVoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussioneVoto_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussioneVoto_Discussione_DiscussioneId",
                        column: x => x.DiscussioneId,
                        principalTable: "Discussione",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussioneVoto_DiscussioneId",
                table: "DiscussioneVoto",
                column: "DiscussioneId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussioneVoto_UtenteId",
                table: "DiscussioneVoto",
                column: "UtenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Articoli_ArticoloId",
                table: "Commenti",
                column: "ArticoloId",
                principalTable: "Articoli",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
