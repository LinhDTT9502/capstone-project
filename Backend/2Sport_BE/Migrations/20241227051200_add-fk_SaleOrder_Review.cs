using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class addfk_SaleOrder_Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReview",
                table: "SaleOrders");

            migrationBuilder.AddColumn<int>(
                name: "SaleOrderId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SaleOrderId",
                table: "Reviews",
                column: "SaleOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_SaleOrders_SaleOrderId",
                table: "Reviews",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "SaleOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_SaleOrders_SaleOrderId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_SaleOrderId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "Reviews");

            migrationBuilder.AddColumn<bool>(
                name: "IsReview",
                table: "SaleOrders",
                type: "bit",
                nullable: true);
        }
    }
}
