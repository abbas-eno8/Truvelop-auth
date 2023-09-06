using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace _1AuthoritySTS.Migrations
{
    public partial class PasswordHistorynew4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPassword_AspNetUsers_AppUserId",
                table: "UsedPassword");

            migrationBuilder.DropIndex(
                name: "IX_UsedPassword_AppUserId",
                table: "UsedPassword");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "UsedPassword");

            migrationBuilder.CreateIndex(
                name: "IX_UsedPassword_UserID",
                table: "UsedPassword",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPassword_AspNetUsers_UserID",
                table: "UsedPassword",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPassword_AspNetUsers_UserID",
                table: "UsedPassword");

            migrationBuilder.DropIndex(
                name: "IX_UsedPassword_UserID",
                table: "UsedPassword");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "UsedPassword",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsedPassword_AppUserId",
                table: "UsedPassword",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPassword_AspNetUsers_AppUserId",
                table: "UsedPassword",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
