using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternconnectBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "UserDetails");
        }
    }
}
