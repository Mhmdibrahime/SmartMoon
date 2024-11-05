using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addrelationbetweeninventoryandsalesbillitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "salesBillItems",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_salesBillItems_InventoryId",
                table: "salesBillItems",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_salesBillItems_inventories_InventoryId",
                table: "salesBillItems",
                column: "InventoryId",
                principalTable: "inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salesBillItems_inventories_InventoryId",
                table: "salesBillItems");

            migrationBuilder.DropIndex(
                name: "IX_salesBillItems_InventoryId",
                table: "salesBillItems");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "salesBillItems");
        }
    }
}
