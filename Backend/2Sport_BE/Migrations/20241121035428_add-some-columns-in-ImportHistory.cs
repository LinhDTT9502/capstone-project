using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class addsomecolumnsinImportHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ImportHistories",
                type: "decimal",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RentPrice",
                table: "ImportHistories",
                type: "decimal",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "ImportHistories");
        }
    }
}
