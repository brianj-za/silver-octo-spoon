using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalLine_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountType", "Balance" },
                values: new object[] { 99999, "Revenue", 0.00m });

            migrationBuilder.CreateIndex(
                name: "IX_JournalLine_JournalEntryId",
                table: "JournalLine",
                column: "JournalEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalLine");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 99999);

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Accounts");
        }
    }
}
