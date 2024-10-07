using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class upadtebbuyill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "billItems",
                newName: "Total");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashPaid",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingBalance",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "billItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_billItems_InventoryId",
                table: "billItems",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_billItems_inventories_InventoryId",
                table: "billItems",
                column: "InventoryId",
                principalTable: "inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_billItems_inventories_InventoryId",
                table: "billItems");

            migrationBuilder.DropIndex(
                name: "IX_billItems_InventoryId",
                table: "billItems");

            migrationBuilder.DropColumn(
                name: "CashPaid",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "RemainingBalance",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "billItems");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "billItems",
                newName: "TotalPrice");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "buyBill",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
