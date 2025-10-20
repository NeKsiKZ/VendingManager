using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VendingManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "LastContact", "Location", "Name", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 20, 11, 55, 0, 0, DateTimeKind.Unspecified), "Hol wejściowy, Budynek A", "Automat GŁÓWNY", "Online" },
                    { 2, new DateTime(2025, 10, 20, 4, 0, 0, 0, DateTimeKind.Unspecified), "Korytarz przy windach", "Automat PIĘTRO 2", "Offline" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Machines",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
