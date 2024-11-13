using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMoon.MVC.Migrations
{
    /// <inheritdoc />
    public partial class addtrasferbetweentables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromMoneyDrawerId = table.Column<int>(type: "int", nullable: false),
                    ToMoneyDrawerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_moneyDrawer_FromMoneyDrawerId",
                        column: x => x.FromMoneyDrawerId,
                        principalTable: "moneyDrawer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Transfers_moneyDrawer_ToMoneyDrawerId",
                        column: x => x.ToMoneyDrawerId,
                        principalTable: "moneyDrawer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromMoneyDrawerId",
                table: "Transfers",
                column: "FromMoneyDrawerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToMoneyDrawerId",
                table: "Transfers",
                column: "ToMoneyDrawerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");
        }
    }
}
