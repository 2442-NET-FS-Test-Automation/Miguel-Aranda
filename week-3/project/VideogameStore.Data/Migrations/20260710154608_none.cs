using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideogameStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class none : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GameStore",
                keyColumn: "Videogame_StoreId",
                keyValue: 1,
                column: "Stock",
                value: 200);

            migrationBuilder.UpdateData(
                table: "GameStore",
                keyColumn: "Videogame_StoreId",
                keyValue: 2,
                column: "Stock",
                value: 120);

            migrationBuilder.InsertData(
                table: "GameStore",
                columns: new[] { "Videogame_StoreId", "Stock", "StoreId", "VideogameId" },
                values: new object[] { 3, 40, 2, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameStore",
                keyColumn: "Videogame_StoreId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "GameStore",
                keyColumn: "Videogame_StoreId",
                keyValue: 1,
                column: "Stock",
                value: 0);

            migrationBuilder.UpdateData(
                table: "GameStore",
                keyColumn: "Videogame_StoreId",
                keyValue: 2,
                column: "Stock",
                value: 0);
        }
    }
}
