using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KodeCrypto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdertxId",
                table: "TradeHistory");

            migrationBuilder.DropColumn(
                name: "PostxId",
                table: "TradeHistory");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ApiKey");

            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "TradeHistory",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "TradeHistory");

            migrationBuilder.AddColumn<string>(
                name: "OrdertxId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostxId",
                table: "TradeHistory",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ApiKey",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
