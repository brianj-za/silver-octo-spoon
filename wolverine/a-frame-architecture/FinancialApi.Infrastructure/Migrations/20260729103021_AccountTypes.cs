using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 99999,
                column: "Type",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "AccountType",
                value: "Credit");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "AccountType",
                value: "Liability");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 99999,
                column: "AccountType",
                value: "Revenue");
        }
    }
}
