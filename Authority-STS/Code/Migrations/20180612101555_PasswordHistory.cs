using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace _1AuthoritySTS.Migrations
{
    public partial class PasswordHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsedPassword",
                columns: table => new
                {
                    HashPassword = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPassword", x => new { x.HashPassword, x.UserID });
                    table.ForeignKey(
                        name: "FK_UsedPassword_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedPassword_AppUserId",
                table: "UsedPassword",
                column: "AppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedPassword");
        }
    }
}
