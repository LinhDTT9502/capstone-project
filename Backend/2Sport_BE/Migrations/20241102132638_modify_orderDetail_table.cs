using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_orderDetail_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_SaleOrders_SaleOrderId",
                table: "OrderDetails",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "SaleOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_SaleOrders_SaleOrderId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "SaleOrderId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_SaleOrders_SaleOrderId",
                table: "OrderDetails",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "SaleOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
