using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class NewLocationstoHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Hotels",
                newName: "Land");

            migrationBuilder.AddColumn<string>(
                name: "By",
                table: "Hotels",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "By",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "Land",
                table: "Hotels",
                newName: "Location");
        }
    }
}
