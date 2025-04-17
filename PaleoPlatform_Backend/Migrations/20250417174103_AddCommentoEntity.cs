using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articoli_AspNetUsers_AutoreId",
                table: "Articoli");

            migrationBuilder.CreateTable(
                name: "Commenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contenuto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPubblicazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    Upvotes = table.Column<int>(type: "int", nullable: false),
                    Downvotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commenti_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Commenti_Commenti_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Commenti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commenti_ParentCommentId",
                table: "Commenti",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commenti_UtenteId",
                table: "Commenti",
                column: "UtenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articoli_AspNetUsers_AutoreId",
                table: "Articoli",
                column: "AutoreId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articoli_AspNetUsers_AutoreId",
                table: "Articoli");

            migrationBuilder.DropTable(
                name: "Commenti");

            migrationBuilder.AddForeignKey(
                name: "FK_Articoli_AspNetUsers_AutoreId",
                table: "Articoli",
                column: "AutoreId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
