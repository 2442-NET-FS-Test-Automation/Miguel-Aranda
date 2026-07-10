using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideogameStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class newEndpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    PromotionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.PromotionId);
                });

            migrationBuilder.CreateTable(
                name: "C_Promotions",
                columns: table => new
                {
                    Customer_PromotionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PromotionId = table.Column<int>(type: "int", nullable: false),
                    AlreadyUsed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_C_Promotions", x => x.Customer_PromotionId);
                    table.ForeignKey(
                        name: "FK_C_Promotions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_C_Promotions_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "PromotionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PromotionId",
                table: "Sales",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_C_Promotions_CustomerId",
                table: "C_Promotions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_C_Promotions_PromotionId",
                table: "C_Promotions",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Promotions_PromotionId",
                table: "Sales",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Promotions_PromotionId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "C_Promotions");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Sales_PromotionId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Sales");
        }
    }
}
