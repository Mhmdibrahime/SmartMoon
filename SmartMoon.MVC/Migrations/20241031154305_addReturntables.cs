using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addReturntables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salesBillItems_inventories_InventoryId",
                table: "salesBillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_salesBillItems_products_ProductId",
                table: "salesBillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_salesBillItems_salesBill_SalesBillId",
                table: "salesBillItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_salesBillItems",
                table: "salesBillItems");

            migrationBuilder.RenameTable(
                name: "salesBillItems",
                newName: "SalesBillItem");

            migrationBuilder.RenameIndex(
                name: "IX_salesBillItems_SalesBillId",
                table: "SalesBillItem",
                newName: "IX_SalesBillItem_SalesBillId");

            migrationBuilder.RenameIndex(
                name: "IX_salesBillItems_ProductId",
                table: "SalesBillItem",
                newName: "IX_SalesBillItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_salesBillItems_InventoryId",
                table: "SalesBillItem",
                newName: "IX_SalesBillItem_InventoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesBillItem",
                table: "SalesBillItem",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "purchaseReturnBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoneyDrawer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseReturnBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_purchaseReturnBills_suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "salesReturnBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CashPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoneyDrawer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salesReturnBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salesReturnBills_clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "purchaseReturnBillItems",
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
                    PurchaseReturnBillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchaseReturnBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_purchaseReturnBillItems_inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchaseReturnBillItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_purchaseReturnBillItems_purchaseReturnBills_PurchaseReturnBillId",
                        column: x => x.PurchaseReturnBillId,
                        principalTable: "purchaseReturnBills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "salesReturnBillItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesBillId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false),
                    SalesReturnBillId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salesReturnBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salesReturnBillItems_inventories_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "inventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_salesReturnBillItems_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_salesReturnBillItems_salesBill_SalesBillId",
                        column: x => x.SalesBillId,
                        principalTable: "salesBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_salesReturnBillItems_salesReturnBills_SalesReturnBillId",
                        column: x => x.SalesReturnBillId,
                        principalTable: "salesReturnBills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_purchaseReturnBillItems_InventoryId",
                table: "purchaseReturnBillItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaseReturnBillItems_ProductId",
                table: "purchaseReturnBillItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaseReturnBillItems_PurchaseReturnBillId",
                table: "purchaseReturnBillItems",
                column: "PurchaseReturnBillId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaseReturnBills_SupplierId",
                table: "purchaseReturnBills",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBillItems_InventoryId",
                table: "salesReturnBillItems",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBillItems_ProductId",
                table: "salesReturnBillItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBillItems_SalesBillId",
                table: "salesReturnBillItems",
                column: "SalesBillId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBillItems_SalesReturnBillId",
                table: "salesReturnBillItems",
                column: "SalesReturnBillId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBills_ClientId",
                table: "salesReturnBills",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesBillItem_inventories_InventoryId",
                table: "SalesBillItem",
                column: "InventoryId",
                principalTable: "inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesBillItem_products_ProductId",
                table: "SalesBillItem",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesBillItem_salesBill_SalesBillId",
                table: "SalesBillItem",
                column: "SalesBillId",
                principalTable: "salesBill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesBillItem_inventories_InventoryId",
                table: "SalesBillItem");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesBillItem_products_ProductId",
                table: "SalesBillItem");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesBillItem_salesBill_SalesBillId",
                table: "SalesBillItem");

            migrationBuilder.DropTable(
                name: "purchaseReturnBillItems");

            migrationBuilder.DropTable(
                name: "salesReturnBillItems");

            migrationBuilder.DropTable(
                name: "purchaseReturnBills");

            migrationBuilder.DropTable(
                name: "salesReturnBills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesBillItem",
                table: "SalesBillItem");

            migrationBuilder.RenameTable(
                name: "SalesBillItem",
                newName: "salesBillItems");

            migrationBuilder.RenameIndex(
                name: "IX_SalesBillItem_SalesBillId",
                table: "salesBillItems",
                newName: "IX_salesBillItems_SalesBillId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesBillItem_ProductId",
                table: "salesBillItems",
                newName: "IX_salesBillItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesBillItem_InventoryId",
                table: "salesBillItems",
                newName: "IX_salesBillItems_InventoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_salesBillItems",
                table: "salesBillItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_salesBillItems_inventories_InventoryId",
                table: "salesBillItems",
                column: "InventoryId",
                principalTable: "inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_salesBillItems_products_ProductId",
                table: "salesBillItems",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_salesBillItems_salesBill_SalesBillId",
                table: "salesBillItems",
                column: "SalesBillId",
                principalTable: "salesBill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
