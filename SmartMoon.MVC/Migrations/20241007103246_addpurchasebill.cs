using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addpurchasebill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "buyTransactions");

            migrationBuilder.DropTable(
                name: "saleTransaction");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "buyBill",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "buyBill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "buyBill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "billItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BuyBillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_billItems_buyBill_BuyBillId",
                        column: x => x.BuyBillId,
                        principalTable: "buyBill",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_billItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryProduct",
                columns: table => new
                {
                    ProductsId = table.Column<int>(type: "int", nullable: false),
                    inventoriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryProduct", x => new { x.ProductsId, x.inventoriesId });
                    table.ForeignKey(
                        name: "FK_InventoryProduct_inventories_inventoriesId",
                        column: x => x.inventoriesId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryProduct_products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_buyBill_SupplierId",
                table: "buyBill",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_billItems_BuyBillId",
                table: "billItems",
                column: "BuyBillId");

            migrationBuilder.CreateIndex(
                name: "IX_billItems_ProductId",
                table: "billItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryProduct_inventoriesId",
                table: "InventoryProduct",
                column: "inventoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_buyBill_suppliers_SupplierId",
                table: "buyBill",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_buyBill_suppliers_SupplierId",
                table: "buyBill");

            migrationBuilder.DropTable(
                name: "billItems");

            migrationBuilder.DropTable(
                name: "InventoryProduct");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DropIndex(
                name: "IX_buyBill_SupplierId",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "buyBill");

            migrationBuilder.CreateTable(
                name: "buyTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buyTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "saleTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saleTransaction", x => x.Id);
                });
        }
    }
}
