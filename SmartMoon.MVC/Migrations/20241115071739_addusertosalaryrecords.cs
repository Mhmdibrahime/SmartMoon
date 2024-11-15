using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addusertosalaryrecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "totalSalaryRecords",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_totalSalaryRecords_UserId",
                table: "totalSalaryRecords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_totalSalaryRecords_AspNetUsers_UserId",
                table: "totalSalaryRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_totalSalaryRecords_AspNetUsers_UserId",
                table: "totalSalaryRecords");

            migrationBuilder.DropIndex(
                name: "IX_totalSalaryRecords_UserId",
                table: "totalSalaryRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "totalSalaryRecords");
        }
    }
}
