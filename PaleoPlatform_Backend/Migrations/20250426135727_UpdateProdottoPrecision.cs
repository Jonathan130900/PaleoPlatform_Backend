using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProdottoPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prodotti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    QuantitaDisponibile = table.Column<int>(type: "int", nullable: false),
                    ImmagineUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prodotti", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prodotti");
        }
    }
}
