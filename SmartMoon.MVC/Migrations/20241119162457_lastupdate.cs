using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class lastupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "expense",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_expense_UserId",
                table: "expense",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_AspNetUsers_UserId",
                table: "expense",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_AspNetUsers_UserId",
                table: "expense");

            migrationBuilder.DropIndex(
                name: "IX_expense_UserId",
                table: "expense");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "expense");
        }
    }
}
