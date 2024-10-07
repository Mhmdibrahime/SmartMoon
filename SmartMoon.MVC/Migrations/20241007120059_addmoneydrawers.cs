using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addmoneydrawers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "moneyDrawer",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "moneyDrawer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MoneyDrawer",
                table: "buyBill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "moneyDrawer");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "moneyDrawer");

            migrationBuilder.DropColumn(
                name: "MoneyDrawer",
                table: "buyBill");
        }
    }
}
