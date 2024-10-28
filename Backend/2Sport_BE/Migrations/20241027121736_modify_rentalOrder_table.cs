using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_rentalOrder_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RentalOrderId",
                table: "RentalOrders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OrderCode",
                table: "RentalOrders",
                newName: "RentalOrderCode");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentStatus",
                table: "RentalOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchName",
                table: "RentalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgAvatarPath",
                table: "RentalOrders",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "RentalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "RentalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "RentalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RentPrice",
                table: "RentalOrders",
                type: "decimal(18,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchName",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ImgAvatarPath",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "RentalOrders");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RentalOrders",
                newName: "RentalOrderId");

            migrationBuilder.RenameColumn(
                name: "RentalOrderCode",
                table: "RentalOrders",
                newName: "OrderCode");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "RentalOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
