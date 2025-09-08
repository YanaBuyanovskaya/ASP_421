using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_421.Migrations
{
    /// <inheritdoc />
    public partial class InitialCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserAccesse",
                keyColumn: "Id",
                keyValue: new Guid("f53c3536-00be-4095-a204-4722196ff284"));

            migrationBuilder.InsertData(
                table: "UserAccesse",
                columns: new[] { "Id", "Dk", "Login", "RoleId", "Salt", "UserId" },
                values: new object[] { new Guid("bbbf2f46-af0a-437c-8082-bac4ae83fe94"), "45A97232E9EE2E8EAE3F", "Admin", "Admin", "B81D9191-7040-41A3-BFBD-6AF1FFB4A266", new Guid("53759101-7de4-4e04-833a-884752290fa0") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("53759101-7de4-4e04-833a-884752290fa0"),
                columns: new[] { "BirthDate", "RegisteredAt" },
                values: new object[] { new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserAccesse",
                keyColumn: "Id",
                keyValue: new Guid("bbbf2f46-af0a-437c-8082-bac4ae83fe94"));

            migrationBuilder.InsertData(
                table: "UserAccesse",
                columns: new[] { "Id", "Dk", "Login", "RoleId", "Salt", "UserId" },
                values: new object[] { new Guid("f53c3536-00be-4095-a204-4722196ff284"), "45A97232E9EE2E8EAE3F", "Admin", "Admin", "B81D9191-7040-41A3-BFBD-6AF1FFB4A266", new Guid("53759101-7de4-4e04-833a-884752290fa0") });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("53759101-7de4-4e04-833a-884752290fa0"),
                columns: new[] { "BirthDate", "RegisteredAt" },
                values: new object[] { new DateTime(2025, 8, 30, 11, 43, 10, 3, DateTimeKind.Local).AddTicks(3490), new DateTime(2025, 8, 30, 11, 43, 10, 12, DateTimeKind.Local).AddTicks(9942) });
        }
    }
}
