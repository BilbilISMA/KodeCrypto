using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KodeCrypto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSyncForService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncedToBinance",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "SyncedToKraken",
                table: "Order",
                newName: "Synced");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Synced",
                table: "Order",
                newName: "SyncedToKraken");

            migrationBuilder.AddColumn<bool>(
                name: "SyncedToBinance",
                table: "Order",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
