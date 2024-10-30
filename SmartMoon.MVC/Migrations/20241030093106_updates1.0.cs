using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class updates10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_moneyDrawer_MoneyDrawerId",
                table: "Expense");

            migrationBuilder.DropTable(
                name: "billItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expense",
                table: "Expense");

            migrationBuilder.RenameTable(
                name: "Expense",
                newName: "expense");

            migrationBuilder.RenameIndex(
                name: "IX_Expense_MoneyDrawerId",
                table: "expense",
                newName: "IX_expense_MoneyDrawerId");

            migrationBuilder.AddColumn<decimal>(
                name: "CashPaid",
                table: "salesBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "salesBill",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "salesBill",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "salesBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MoneyDrawer",
                table: "salesBill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingBalance",
                table: "salesBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "salesBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_expense",
                table: "expense",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "buyBillItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    BuyBillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buyBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_buyBillItems_buyBill_BuyBillId",
                        column: x => x.BuyBillId,
                        principalTable: "buyBill",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_buyBillItems_inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_buyBillItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventoryProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventoryProducts", x => new { x.ProductId, x.InventoryId });
                    table.ForeignKey(
                        name: "FK_inventoryProducts_inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventoryProducts_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "salesBillItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesBillId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salesBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salesBillItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_salesBillItems_salesBill_SalesBillId",
                        column: x => x.SalesBillId,
                        principalTable: "salesBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_salesBill_ClientId",
                table: "salesBill",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_buyBillItems_BuyBillId",
                table: "buyBillItems",
                column: "BuyBillId");

            migrationBuilder.CreateIndex(
                name: "IX_buyBillItems_InventoryId",
                table: "buyBillItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_buyBillItems_ProductId",
                table: "buyBillItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_inventoryProducts_InventoryId",
                table: "inventoryProducts",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_salesBillItems_ProductId",
                table: "salesBillItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_salesBillItems_SalesBillId",
                table: "salesBillItems",
                column: "SalesBillId");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_moneyDrawer_MoneyDrawerId",
                table: "expense",
                column: "MoneyDrawerId",
                principalTable: "moneyDrawer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_salesBill_clients_ClientId",
                table: "salesBill",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_moneyDrawer_MoneyDrawerId",
                table: "expense");

            migrationBuilder.DropForeignKey(
                name: "FK_salesBill_clients_ClientId",
                table: "salesBill");

            migrationBuilder.DropTable(
                name: "buyBillItems");

            migrationBuilder.DropTable(
                name: "inventoryProducts");

            migrationBuilder.DropTable(
                name: "salesBillItems");

            migrationBuilder.DropIndex(
                name: "IX_salesBill_ClientId",
                table: "salesBill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_expense",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "CashPaid",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "MoneyDrawer",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "RemainingBalance",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "salesBill");

            migrationBuilder.RenameTable(
                name: "expense",
                newName: "Expense");

            migrationBuilder.RenameIndex(
                name: "IX_expense_MoneyDrawerId",
                table: "Expense",
                newName: "IX_Expense_MoneyDrawerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expense",
                table: "Expense",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "billItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    BuyBillId = table.Column<int>(type: "int", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                        name: "FK_billItems_inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_billItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_billItems_BuyBillId",
                table: "billItems",
                column: "BuyBillId");

            migrationBuilder.CreateIndex(
                name: "IX_billItems_InventoryId",
                table: "billItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_billItems_ProductId",
                table: "billItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryProduct_inventoriesId",
                table: "InventoryProduct",
                column: "inventoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_moneyDrawer_MoneyDrawerId",
                table: "Expense",
                column: "MoneyDrawerId",
                principalTable: "moneyDrawer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
