using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace _1AuthoritySTS.Migrations
{
    public partial class PasswordHistorynew2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedPassword",
                table: "UsedPassword");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UsedPassword",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedPassword",
                table: "UsedPassword",
                column: "HashPassword");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsedPassword",
                table: "UsedPassword");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UsedPassword",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsedPassword",
                table: "UsedPassword",
                columns: new[] { "HashPassword", "UserID" });
        }
    }
}
