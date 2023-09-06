using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace _1AuthoritySTS.Migrations
{
    public partial class PasswordHistory3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedPassword");

            migrationBuilder.CreateTable(
                name: "PreviousPassword",
                columns: table => new
                {
                    PasswordHash = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreviousPassword", x => new { x.PasswordHash, x.UserId });
                    table.ForeignKey(
                        name: "FK_PreviousPassword_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreviousPassword_UserId",
                table: "PreviousPassword",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreviousPassword");

            migrationBuilder.CreateTable(
                name: "UsedPassword",
                columns: table => new
                {
                    HashPassword = table.Column<string>(nullable: false),
                    UserID = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedPassword", x => new { x.HashPassword, x.UserID });
                    table.ForeignKey(
                        name: "FK_UsedPassword_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedPassword_Id",
                table: "UsedPassword",
                column: "Id");
        }
    }
}
