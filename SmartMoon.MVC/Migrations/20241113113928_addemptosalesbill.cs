using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addemptosalesbill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "salesBill",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_salesBill_EmployeeId",
                table: "salesBill",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_salesBill_employees_EmployeeId",
                table: "salesBill",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salesBill_employees_EmployeeId",
                table: "salesBill");

            migrationBuilder.DropIndex(
                name: "IX_salesBill_EmployeeId",
                table: "salesBill");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "salesBill");
        }
    }
}
