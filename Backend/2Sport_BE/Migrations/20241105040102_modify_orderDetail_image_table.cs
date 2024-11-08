using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_orderDetail_image_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfReceipt",
                table: "RentalOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryMethod",
                table: "RentalOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "RentalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "RentalOrders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgAvatarPath",
                table: "OrderDetails",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfReceipt",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ImgAvatarPath",
                table: "OrderDetails");
        }
    }
}
