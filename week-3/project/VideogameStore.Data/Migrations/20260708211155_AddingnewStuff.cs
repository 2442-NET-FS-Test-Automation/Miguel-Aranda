using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideogameStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingnewStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Employees_EmployeeId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_EmployeeId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Stores");

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "StoreId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "StoreId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StoreId",
                table: "Employees",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Stores_StoreId",
                table: "Employees",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Stores_StoreId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_StoreId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "StoreId",
                keyValue: 1,
                column: "EmployeeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Stores",
                keyColumn: "StoreId",
                keyValue: 2,
                column: "EmployeeId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_EmployeeId",
                table: "Stores",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Employees_EmployeeId",
                table: "Stores",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
