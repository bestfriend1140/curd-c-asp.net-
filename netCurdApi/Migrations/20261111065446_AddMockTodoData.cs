using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace netCurdApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMockTodoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "CreatedAt", "Description", "IsCompleted", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 11, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), "Learn Web API and Entity Framework Core", false, "Learn ASP.NET Core" },
                    { 2, new DateTime(2026, 11, 11, 9, 30, 0, 0, DateTimeKind.Unspecified), "Learn Angular components and services", false, "Learn Angular" },
                    { 3, new DateTime(2026, 11, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), "Implement Create, Read, Update and Delete", true, "Create CRUD API" },
                    { 4, new DateTime(2026, 11, 11, 10, 30, 0, 0, DateTimeKind.Unspecified), "Upload source code and migrations", false, "Push Project to GitHub" },
                    { 5, new DateTime(2026, 11, 11, 11, 0, 0, 0, DateTimeKind.Unspecified), "Test all CRUD endpoints", true, "Test API with Postman" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TodoItems",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
