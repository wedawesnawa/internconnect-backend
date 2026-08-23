using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternconnectBackend.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Logbooks",
                columns: table => new
                {
                    KodeLogbook = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deskripsi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logbooks", x => x.KodeLogbook);
                    table.ForeignKey(
                        name: "FK_Logbooks_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instansi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlamatInstansi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDetails_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogbookDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deskripsi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kendala = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusAttendant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeLogbook = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogbookDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogbookDetails_Logbooks_KodeLogbook",
                        column: x => x.KodeLogbook,
                        principalTable: "Logbooks",
                        principalColumn: "KodeLogbook",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogbookShareds",
                columns: table => new
                {
                    IdShared = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeLogbook = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SharedWith = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SharedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Permission = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogbookShareds", x => x.IdShared);
                    table.ForeignKey(
                        name: "FK_LogbookShareds_Logbooks_KodeLogbook",
                        column: x => x.KodeLogbook,
                        principalTable: "Logbooks",
                        principalColumn: "KodeLogbook",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogbookShareds_Users_SharedWith",
                        column: x => x.SharedWith,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Monevs",
                columns: table => new
                {
                    IdMonev = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    roomUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KodeLogbook = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monevs", x => x.IdMonev);
                    table.ForeignKey(
                        name: "FK_Monevs_Logbooks_KodeLogbook",
                        column: x => x.KodeLogbook,
                        principalTable: "Logbooks",
                        principalColumn: "KodeLogbook",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogbookDetails_KodeLogbook",
                table: "LogbookDetails",
                column: "KodeLogbook");

            migrationBuilder.CreateIndex(
                name: "IX_Logbooks_Username",
                table: "Logbooks",
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
                name: "IX_Monevs_KodeLogbook",
                table: "Monevs",
                column: "KodeLogbook");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_Username",
                table: "UserDetails",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogbookDetails");

            migrationBuilder.DropTable(
                name: "LogbookShareds");

            migrationBuilder.DropTable(
                name: "Monevs");

            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "Logbooks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
