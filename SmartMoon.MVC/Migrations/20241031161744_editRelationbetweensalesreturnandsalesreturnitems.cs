using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class editRelationbetweensalesreturnandsalesreturnitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salesReturnBillItems_salesBill_SalesBillId",
                table: "salesReturnBillItems");

            migrationBuilder.DropForeignKey(
                name: "FK_salesReturnBillItems_salesReturnBills_SalesReturnBillId",
                table: "salesReturnBillItems");

            migrationBuilder.DropIndex(
                name: "IX_salesReturnBillItems_SalesBillId",
                table: "salesReturnBillItems");

            migrationBuilder.DropColumn(
                name: "SalesBillId",
                table: "salesReturnBillItems");

            migrationBuilder.AlterColumn<int>(
                name: "SalesReturnBillId",
                table: "salesReturnBillItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_salesReturnBillItems_salesReturnBills_SalesReturnBillId",
                table: "salesReturnBillItems",
                column: "SalesReturnBillId",
                principalTable: "salesReturnBills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salesReturnBillItems_salesReturnBills_SalesReturnBillId",
                table: "salesReturnBillItems");

            migrationBuilder.AlterColumn<int>(
                name: "SalesReturnBillId",
                table: "salesReturnBillItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SalesBillId",
                table: "salesReturnBillItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBillItems_SalesBillId",
                table: "salesReturnBillItems",
                column: "SalesBillId");

            migrationBuilder.AddForeignKey(
                name: "FK_salesReturnBillItems_salesBill_SalesBillId",
                table: "salesReturnBillItems",
                column: "SalesBillId",
                principalTable: "salesBill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_salesReturnBillItems_salesReturnBills_SalesReturnBillId",
                table: "salesReturnBillItems",
                column: "SalesReturnBillId",
                principalTable: "salesReturnBills",
                principalColumn: "Id");
        }
    }
}
