using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscussioneId",
                table: "Commenti",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discussione",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titolo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contenuto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AutoreId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: false),
                    Upvotes = table.Column<int>(type: "int", nullable: false),
                    Downvotes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discussione", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discussione_AspNetUsers_AutoreId",
                        column: x => x.AutoreId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discussione_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussioneVoto_Discussione_DiscussioneId",
                        column: x => x.DiscussioneId,
                        principalTable: "Discussione",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commenti_DiscussioneId",
                table: "Commenti",
                column: "DiscussioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussione_AutoreId",
                table: "Discussione",
                column: "AutoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussione_TopicId",
                table: "Discussione",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussioneVoto_DiscussioneId",
                table: "DiscussioneVoto",
                column: "DiscussioneId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussioneVoto_UtenteId",
                table: "DiscussioneVoto",
                column: "UtenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commenti_Discussione_DiscussioneId",
                table: "Commenti",
                column: "DiscussioneId",
                principalTable: "Discussione",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commenti_Discussione_DiscussioneId",
                table: "Commenti");

            migrationBuilder.DropTable(
                name: "DiscussioneVoto");

            migrationBuilder.DropTable(
                name: "Discussione");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Commenti_DiscussioneId",
                table: "Commenti");

            migrationBuilder.DropColumn(
                name: "DiscussioneId",
                table: "Commenti");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");
        }
    }
}
