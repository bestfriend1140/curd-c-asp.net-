using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace netCurdApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Status", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 11, 11, 8, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7", "A", "admin" },
                    { 2, new DateTime(2026, 11, 11, 8, 15, 0, 0, DateTimeKind.Unspecified), "alice@example.com", "3d5e9ac865d0e0ec9a56fa0f8739772b001269721c5776eb4dfe25c1eab1919f", "A", "alice" },
                    { 3, new DateTime(2026, 11, 11, 8, 30, 0, 0, DateTimeKind.Unspecified), "bob@example.com", "fc6e5ceb0061b7059f2d38619c03c7b2f4f1ca6e0d29be5b2c0ea7f7d903c197", "I", "bob" }
                });

            migrationBuilder.InsertData(
                table: "TodoItems",
                columns: new[] { "Id", "CreatedAt", "Description", "IsCompleted", "Status", "Title", "UserId" },
                values: new object[,]
                {
                    { "todo_01", new DateTime(2026, 11, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), "Learn Web API and Entity Framework Core", false, "A", "Learn ASP.NET Core", 1 },
                    { "todo_02", new DateTime(2026, 11, 11, 9, 30, 0, 0, DateTimeKind.Unspecified), "Learn Angular components and services", false, "A", "Learn Angular", 1 },
                    { "todo_03", new DateTime(2026, 11, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), "Implement Create, Read, Update and Delete", true, "A", "Create CRUD API", 2 },
                    { "todo_04", new DateTime(2026, 11, 11, 10, 30, 0, 0, DateTimeKind.Unspecified), "Upload source code and migrations", false, "A", "Push Project to GitHub", 2 },
                    { "todo_05", new DateTime(2026, 11, 11, 11, 0, 0, 0, DateTimeKind.Unspecified), "Test all CRUD endpoints", true, "A", "Test API with Postman", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_UserId",
                table: "TodoItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
