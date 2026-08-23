using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternconnectBackend.Migrations
{
    /// <inheritdoc />
    public partial class Updatemonevtb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogbookDetails_Logbooks_KodeLogbook",
                table: "LogbookDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LogbookShareds_Logbooks_KodeLogbook",
                table: "LogbookShareds");

            migrationBuilder.DropForeignKey(
                name: "FK_LogbookShareds_Users_SharedWith",
                table: "LogbookShareds");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_Users_Username",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_Username",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_LogbookShareds_KodeLogbook",
                table: "LogbookShareds");

            migrationBuilder.DropIndex(
                name: "IX_LogbookShareds_SharedWith",
                table: "LogbookShareds");

            migrationBuilder.DropIndex(
                name: "IX_LogbookDetails_KodeLogbook",
                table: "LogbookDetails");

            migrationBuilder.RenameColumn(
                name: "roomUrl",
                table: "Monevs",
                newName: "RoomUrl");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "LogbookDetails",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "StatusAttendant",
                table: "LogbookDetails",
                newName: "StatusAttend");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Telp",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nama",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Instansi",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Alamat",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdShared",
                table: "Monevs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SharedWith",
                table: "LogbookShareds",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monevs_IdShared",
                table: "Monevs",
                column: "IdShared");

            migrationBuilder.AddForeignKey(
                name: "FK_Monevs_LogbookShareds_IdShared",
                table: "Monevs",
                column: "IdShared",
                principalTable: "LogbookShareds",
                principalColumn: "IdShared",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monevs_LogbookShareds_IdShared",
                table: "Monevs");

            migrationBuilder.DropIndex(
                name: "IX_Monevs_IdShared",
                table: "Monevs");

            migrationBuilder.DropColumn(
                name: "IdShared",
                table: "Monevs");

            migrationBuilder.RenameColumn(
                name: "RoomUrl",
                table: "Monevs",
                newName: "roomUrl");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "LogbookDetails",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "StatusAttend",
                table: "LogbookDetails",
                newName: "StatusAttendant");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserDetails",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Telp",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nama",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instansi",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Alamat",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SharedWith",
                table: "LogbookShareds",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_Username",
                table: "UserDetails",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_LogbookShareds_KodeLogbook",
                table: "LogbookShareds",
                column: "KodeLogbook");

            migrationBuilder.CreateIndex(
                name: "IX_LogbookShareds_SharedWith",
                table: "LogbookShareds",
                column: "SharedWith");

            migrationBuilder.CreateIndex(
                name: "IX_LogbookDetails_KodeLogbook",
                table: "LogbookDetails",
                column: "KodeLogbook");

            migrationBuilder.AddForeignKey(
                name: "FK_LogbookDetails_Logbooks_KodeLogbook",
                table: "LogbookDetails",
                column: "KodeLogbook",
                principalTable: "Logbooks",
                principalColumn: "KodeLogbook",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogbookShareds_Logbooks_KodeLogbook",
                table: "LogbookShareds",
                column: "KodeLogbook",
                principalTable: "Logbooks",
                principalColumn: "KodeLogbook",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogbookShareds_Users_SharedWith",
                table: "LogbookShareds",
                column: "SharedWith",
                principalTable: "Users",
                principalColumn: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_Users_Username",
                table: "UserDetails",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
