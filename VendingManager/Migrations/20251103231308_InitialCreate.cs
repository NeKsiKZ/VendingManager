using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VendingManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    LastContact = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineErrorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineErrorLogs_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineSlots_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MachineSlots_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "LastContact", "Location", "Name", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 20, 11, 55, 0, 0, DateTimeKind.Unspecified), "Hol wejściowy, Budynek A", "Automat GŁÓWNY", "Online" },
                    { 2, new DateTime(2025, 10, 20, 4, 0, 0, 0, DateTimeKind.Unspecified), "Korytarz przy windach", "Automat PIĘTRO 2", "Offline" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 2, "Seed Data", "Fanta (Test)", 3.00m },
                    { 3, "Seed Data", "Cola (Test)", 3.50m },
                    { 4, "Seed Data", "Woda (Test)", 3m },
                    { 10, "Seed Data", "Monster (Test)", 7m }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineErrorLogs_MachineId",
                table: "MachineErrorLogs",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineSlots_MachineId",
                table: "MachineSlots",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineSlots_ProductId",
                table: "MachineSlots",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MachineId",
                table: "Transactions",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ProductId",
                table: "Transactions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "MachineErrorLogs");

            migrationBuilder.DropTable(
                name: "MachineSlots");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
