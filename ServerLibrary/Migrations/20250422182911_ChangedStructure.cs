using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(251), new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(252) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(8343));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(8503));

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1194), new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1194) });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1655), new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1655) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(708));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(9254));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(9962));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1849), new DateTime(2025, 4, 22, 18, 29, 10, 677, DateTimeKind.Utc).AddTicks(1850) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "UpdatedAt" },
                values: new object[] { "$2a$11$KlmfsQMS9aHuU56jghzUCeRj3Y5L8M07j6apT14Tlh27QXr6wFi3K", new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(2649) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "UpdatedAt" },
                values: new object[] { "$2a$11$UFGyAKF2jCbnBtaGgz9mVOX4Fev1WABX6r7PVZJ3oZDWxdtPqXkWy", new DateTime(2025, 4, 22, 18, 29, 10, 676, DateTimeKind.Utc).AddTicks(3957) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(8580), new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(8580) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(5935));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(6363));

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(9504), new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(9505) });

            migrationBuilder.UpdateData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 20, 56, 536, DateTimeKind.Utc).AddTicks(407), new DateTime(2025, 4, 22, 18, 20, 56, 536, DateTimeKind.Utc).AddTicks(409) });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(9020));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(7275));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2025, 4, 22, 18, 20, 56, 535, DateTimeKind.Utc).AddTicks(8353));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 4, 22, 18, 20, 56, 536, DateTimeKind.Utc).AddTicks(558), new DateTime(2025, 4, 22, 18, 20, 56, 536, DateTimeKind.Utc).AddTicks(559) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "UpdatedAt" },
                values: new object[] { "$2a$11$5dSx8UFUoPC2lpwDVLK/D.dmgYdEzD92pTM8/Cvz95B1O56uNWrFm", new DateTime(2025, 4, 22, 18, 20, 56, 254, DateTimeKind.Utc).AddTicks(8418) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "UpdatedAt" },
                values: new object[] { "$2a$11$uUZwBHm7mzvjlX7xY1w./OONVFjM701Z1dlDUIFpfM4i9QMk2wJYy", new DateTime(2025, 4, 22, 18, 20, 56, 422, DateTimeKind.Utc).AddTicks(1571) });
        }
    }
}
