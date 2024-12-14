using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class changeproductidtoproductcodeinLikeCommentReviewtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Products_ProductId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ProductId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Reviews",
                type: "nvarchar(15)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Likes",
                type: "nvarchar(15)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Comments",
                type: "nvarchar(15)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProductId",
                table: "Comments",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Products_ProductId",
                table: "Comments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
