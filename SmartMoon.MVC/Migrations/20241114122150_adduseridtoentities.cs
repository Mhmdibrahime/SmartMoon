using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class adduseridtoentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Transfers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "supplierReceipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "salesReturnBills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "salesBill",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "purchaseReturnBills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "clientReceipts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "buyBill",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_UserId",
                table: "Transfers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_supplierReceipts_UserId",
                table: "supplierReceipts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_salesReturnBills_UserId",
                table: "salesReturnBills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_salesBill_UserId",
                table: "salesBill",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_purchaseReturnBills_UserId",
                table: "purchaseReturnBills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_clientReceipts_UserId",
                table: "clientReceipts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_buyBill_UserId",
                table: "buyBill",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_buyBill_AspNetUsers_UserId",
                table: "buyBill",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
                );

            migrationBuilder.AddForeignKey(
                name: "FK_clientReceipts_AspNetUsers_UserId",
                table: "clientReceipts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaseReturnBills_AspNetUsers_UserId",
                table: "purchaseReturnBills",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_salesBill_AspNetUsers_UserId",
                table: "salesBill",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_salesReturnBills_AspNetUsers_UserId",
                table: "salesReturnBills",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_supplierReceipts_AspNetUsers_UserId",
                table: "supplierReceipts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_AspNetUsers_UserId",
                table: "Transfers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_buyBill_AspNetUsers_UserId",
                table: "buyBill");

            migrationBuilder.DropForeignKey(
                name: "FK_clientReceipts_AspNetUsers_UserId",
                table: "clientReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_purchaseReturnBills_AspNetUsers_UserId",
                table: "purchaseReturnBills");

            migrationBuilder.DropForeignKey(
                name: "FK_salesBill_AspNetUsers_UserId",
                table: "salesBill");

            migrationBuilder.DropForeignKey(
                name: "FK_salesReturnBills_AspNetUsers_UserId",
                table: "salesReturnBills");

            migrationBuilder.DropForeignKey(
                name: "FK_supplierReceipts_AspNetUsers_UserId",
                table: "supplierReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_AspNetUsers_UserId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_UserId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_supplierReceipts_UserId",
                table: "supplierReceipts");

            migrationBuilder.DropIndex(
                name: "IX_salesReturnBills_UserId",
                table: "salesReturnBills");

            migrationBuilder.DropIndex(
                name: "IX_salesBill_UserId",
                table: "salesBill");

            migrationBuilder.DropIndex(
                name: "IX_purchaseReturnBills_UserId",
                table: "purchaseReturnBills");

            migrationBuilder.DropIndex(
                name: "IX_clientReceipts_UserId",
                table: "clientReceipts");

            migrationBuilder.DropIndex(
                name: "IX_buyBill_UserId",
                table: "buyBill");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "supplierReceipts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "salesReturnBills");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "purchaseReturnBills");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "clientReceipts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "buyBill");
        }
    }
}
