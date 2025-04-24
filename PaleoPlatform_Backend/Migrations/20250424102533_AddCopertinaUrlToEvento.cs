using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaleoPlatform_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCopertinaUrlToEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CopertinaUrl",
                table: "Eventi",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CopertinaUrl",
                table: "Eventi");
        }
    }
}
