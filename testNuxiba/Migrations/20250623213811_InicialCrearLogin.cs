using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace testNuxiba.Migrations
{
    /// <inheritdoc />
    public partial class InicialCrearLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ccRIACat_Areas",
                columns: table => new
                {
                    IDArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusArea = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccRIACat_Areas", x => x.IDArea);
                });

            migrationBuilder.CreateTable(
                name: "ccUsers",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUser_id = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    fCreate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IDArea = table.Column<int>(type: "int", nullable: true),
                    LastLoginAttempt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccUsers", x => x.User_id);
                    table.ForeignKey(
                        name: "FK_ccUsers_ccRIACat_Areas_IDArea",
                        column: x => x.IDArea,
                        principalTable: "ccRIACat_Areas",
                        principalColumn: "IDArea");
                });

            migrationBuilder.CreateTable(
                name: "ccLogLogin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_id = table.Column<int>(type: "int", nullable: false),
                    Extension = table.Column<int>(type: "int", nullable: true),
                    TipoMov = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccLogLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ccLogLogin_ccUsers_User_id",
                        column: x => x.User_id,
                        principalTable: "ccUsers",
                        principalColumn: "User_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ccLogLogin_User_id",
                table: "ccLogLogin",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_ccUsers_IDArea",
                table: "ccUsers",
                column: "IDArea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ccLogLogin");

            migrationBuilder.DropTable(
                name: "ccUsers");

            migrationBuilder.DropTable(
                name: "ccRIACat_Areas");
        }
    }
}
