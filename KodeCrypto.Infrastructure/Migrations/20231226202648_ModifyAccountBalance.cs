using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KodeCrypto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAccountBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "AccountBalance");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "AccountBalance",
                newName: "Data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "AccountBalance",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "AccountBalance",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
