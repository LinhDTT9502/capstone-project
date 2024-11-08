using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_saleOrder_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderDetails",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfReceipt",
                table: "SaleOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "SaleOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "SaleOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsExtendRentalOrder",
                table: "RentalOrders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentOrderCode",
                table: "RentalOrders",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfReceipt",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "IsExtendRentalOrder",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ParentOrderCode",
                table: "RentalOrders");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderDetails",
                newName: "Price");
        }
    }
}
