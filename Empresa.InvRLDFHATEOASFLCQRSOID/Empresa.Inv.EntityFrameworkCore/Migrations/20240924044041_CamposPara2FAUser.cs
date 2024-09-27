using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empresa.Inv.EntityFrameworkCore.Migrations
{
    public partial class CamposPara2FAUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFactorCode",
                table: "tbUser",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorExpiry",
                table: "tbUser",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorCode",
                table: "tbUser");

            migrationBuilder.DropColumn(
                name: "TwoFactorExpiry",
                table: "tbUser");
        }
    }
}
