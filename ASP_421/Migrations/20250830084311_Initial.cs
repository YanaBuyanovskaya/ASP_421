using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_421.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dk = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesse_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesse_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Description" },
                values: new object[,]
                {
                    { "Admin", true, true, true, true, "Root System Administation" },
                    { "Editor", false, true, true, true, "Content Editor and Moderator" },
                    { "Guest", false, false, false, false, "Self Registered User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "DeletedAt", "Email", "Name", "RegisteredAt" },
                values: new object[] { new Guid("53759101-7de4-4e04-833a-884752290fa0"), new DateTime(2025, 8, 30, 11, 43, 10, 3, DateTimeKind.Local).AddTicks(3490), null, "admin@i.ua", "Root Administrator", new DateTime(2025, 8, 30, 11, 43, 10, 12, DateTimeKind.Local).AddTicks(9942) });

            migrationBuilder.InsertData(
                table: "UserAccesse",
                columns: new[] { "Id", "Dk", "Login", "RoleId", "Salt", "UserId" },
                values: new object[] { new Guid("f53c3536-00be-4095-a204-4722196ff284"), "45A97232E9EE2E8EAE3F", "Admin", "Admin", "B81D9191-7040-41A3-BFBD-6AF1FFB4A266", new Guid("53759101-7de4-4e04-833a-884752290fa0") });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesse_Login",
                table: "UserAccesse",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesse_RoleId",
                table: "UserAccesse",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesse_UserId",
                table: "UserAccesse",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccesse");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
