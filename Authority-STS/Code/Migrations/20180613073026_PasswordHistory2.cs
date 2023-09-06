using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace _1AuthoritySTS.Migrations
{
    public partial class PasswordHistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPassword_AspNetUsers_UserID",
                table: "UsedPassword");

            migrationBuilder.DropIndex(
                name: "IX_UsedPassword_UserID",
                table: "UsedPassword");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UsedPassword",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsedPassword_Id",
                table: "UsedPassword",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsedPassword_AspNetUsers_Id",
                table: "UsedPassword",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsedPassword_AspNetUsers_Id",
                table: "UsedPassword");

            migrationBuilder.DropIndex(
                name: "IX_UsedPassword_Id",
                table: "UsedPassword");

            migrationBuilder.DropColumn(
                name: "Id",
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
    }
}
