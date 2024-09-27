using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Empresa.Inv.EntityFrameworkCore.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbSuppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbSuppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbProducts_tbCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tbCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_tbProducts_tbSuppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "tbSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Token);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_tbUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tbUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "tbBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbBalances_tbProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tbProducts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tbBalances_tbUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tbUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tbKardexs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbKardexs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbKardexs_tbProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "tbProducts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tbKardexs_tbUser_UserId",
                        column: x => x.UserId,
                        principalTable: "tbUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbBalances_ProductId",
                table: "tbBalances",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tbBalances_UserId",
                table: "tbBalances",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbKardexs_ProductId",
                table: "tbKardexs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_tbKardexs_UserId",
                table: "tbKardexs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbProducts_CategoryId",
                table: "tbProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tbProducts_SupplierId",
                table: "tbProducts",
                column: "SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "tbBalances");

            migrationBuilder.DropTable(
                name: "tbKardexs");

            migrationBuilder.DropTable(
                name: "tbProducts");

            migrationBuilder.DropTable(
                name: "tbUser");

            migrationBuilder.DropTable(
                name: "tbCategories");

            migrationBuilder.DropTable(
                name: "tbSuppliers");
        }
    }
}
