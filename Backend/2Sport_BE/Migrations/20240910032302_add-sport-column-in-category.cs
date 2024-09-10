using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class addsportcolumnincategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SportId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SportId",
                table: "Categories",
                column: "SportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Sports_SportId",
                table: "Categories",
                column: "SportId",
                principalTable: "Sports",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Sports_SportId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SportId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SportId",
                table: "Categories");
        }
    }
}
