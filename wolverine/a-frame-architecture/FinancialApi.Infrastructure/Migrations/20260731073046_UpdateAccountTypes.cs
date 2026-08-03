using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 99999,
                column: "Type",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 99999,
                column: "Type",
                value: 3);
        }
    }
}
