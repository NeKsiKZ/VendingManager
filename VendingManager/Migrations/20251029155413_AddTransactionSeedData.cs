using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VendingManager.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "MachineId", "ProductId", "SalePrice", "TransactionDate" },
                values: new object[,]
                {
                    { 101, 1, 3, 3.50m, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 102, 2, 2, 3.00m, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 103, 1, 4, 3.00m, new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 104, 1, 3, 3.50m, new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 105, 1, 10, 3.50m, new DateTime(2025, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 106, 2, 4, 3.50m, new DateTime(2025, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 107, 1, 2, 3.00m, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 108, 2, 4, 3.50m, new DateTime(2025, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 109, 2, 10, 3.00m, new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 110, 1, 2, 3.00m, new DateTime(2025, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 111, 2, 3, 3.50m, new DateTime(2025, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 112, 1, 4, 3.50m, new DateTime(2025, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 113, 2, 10, 3.00m, new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 114, 1, 2, 3.00m, new DateTime(2025, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 115, 1, 3, 3.50m, new DateTime(2025, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 116, 2, 4, 3.50m, new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 117, 1, 10, 3.00m, new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 118, 2, 2, 3.00m, new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 119, 1, 3, 3.50m, new DateTime(2025, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 120, 2, 4, 3.50m, new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 121, 1, 10, 3.00m, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 122, 2, 2, 3.00m, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 123, 1, 3, 3.50m, new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 124, 2, 10, 3.00m, new DateTime(2025, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 125, 1, 4, 3.50m, new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 126, 2, 2, 3.00m, new DateTime(2025, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 127, 1, 3, 3.50m, new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 128, 2, 10, 3.00m, new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 129, 1, 4, 3.50m, new DateTime(2025, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 130, 2, 2, 3.00m, new DateTime(2025, 5, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 131, 2, 10, 3.00m, new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 132, 2, 2, 3.50m, new DateTime(2025, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 133, 2, 4, 3.50m, new DateTime(2025, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 134, 2, 3, 3.50m, new DateTime(2025, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 134);
        }
    }
}
