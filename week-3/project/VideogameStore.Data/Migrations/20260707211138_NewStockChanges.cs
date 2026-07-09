using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideogameStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewStockChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Game");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "GameStore",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "GameStore",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "GameStore");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "GameStore");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Game",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Game",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Game",
                keyColumn: "VideogameId",
                keyValue: 1,
                column: "Stock",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Game",
                keyColumn: "VideogameId",
                keyValue: 2,
                column: "Stock",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Game",
                keyColumn: "VideogameId",
                keyValue: 3,
                column: "Stock",
                value: 5);
        }
    }
}
