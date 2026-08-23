using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternconnectBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalDateRange",
                table: "Logbooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLogbookDetails",
                table: "Logbooks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDateRange",
                table: "Logbooks");

            migrationBuilder.DropColumn(
                name: "TotalLogbookDetails",
                table: "Logbooks");
        }
    }
}
