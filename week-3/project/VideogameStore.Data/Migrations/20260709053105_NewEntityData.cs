using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VideogameStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewEntityData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Customers",
                newName: "Country");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "Country", "Email", "Name", "SurName" },
                values: new object[,]
                {
                    { 1, "El viejo jenkins", "Mexico", "Jorge@example.com", "Jorge", "Bustamante" },
                    { 2, "Halmart street 3", "USA", "Martha@example.com", "Martha", "Gonzales" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentMethodId", "MethodName" },
                values: new object[,]
                {
                    { 1, "Credit Card" },
                    { 2, "Debit Card" },
                    { 3, "Cash" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentMethodId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentMethodId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentMethodId",
                keyValue: 3);

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Customers",
                newName: "City");
        }
    }
}
