using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamma_Pasta.Migrations
{
    /// <inheritdoc />
    public partial class ventas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Ventas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Ventas");
        }
    }
}
