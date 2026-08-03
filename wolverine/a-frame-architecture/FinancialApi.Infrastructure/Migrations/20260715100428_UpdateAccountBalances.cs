using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountBalances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Balance",
                value: 10000000.00m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Balance",
                value: 50000000.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Balance",
                value: 100.00m);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Balance",
                value: 500.00m);
        }
    }
}
