using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_rentalOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_RentalOrders_RentalOrderID",
                table: "RefundRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_SaleOrders_SaleOrderID",
                table: "RefundRequests");

            migrationBuilder.RenameColumn(
                name: "IsExtendRentalOrder",
                table: "RentalOrders",
                newName: "IsExtended");

            migrationBuilder.AddColumn<decimal>(
                name: "DepositAmount",
                table: "RentalOrders",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepositStatus",
                table: "RentalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtensionCost",
                table: "RentalOrders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtensionDays",
                table: "RentalOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentalDays",
                table: "RentalOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "StaffNotes",
                table: "RefundRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "SaleOrderID",
                table: "RefundRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RentalOrderID",
                table: "RefundRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RefundMethod",
                table: "RefundRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundAmount",
                table: "RefundRequests",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_RentalOrders_RentalOrderID",
                table: "RefundRequests",
                column: "RentalOrderID",
                principalTable: "RentalOrders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_SaleOrders_SaleOrderID",
                table: "RefundRequests",
                column: "SaleOrderID",
                principalTable: "SaleOrders",
                principalColumn: "SaleOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_RentalOrders_RentalOrderID",
                table: "RefundRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_RefundRequests_SaleOrders_SaleOrderID",
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "DepositAmount",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "DepositStatus",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ExtensionCost",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "ExtensionDays",
                table: "RentalOrders");

            migrationBuilder.DropColumn(
                name: "RentalDays",
                table: "RentalOrders");

            migrationBuilder.RenameColumn(
                name: "IsExtended",
                table: "RentalOrders",
                newName: "IsExtendRentalOrder");

            migrationBuilder.AlterColumn<string>(
                name: "StaffNotes",
                table: "RefundRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SaleOrderID",
                table: "RefundRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RentalOrderID",
                table: "RefundRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefundMethod",
                table: "RefundRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RefundAmount",
                table: "RefundRequests",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_RentalOrders_RentalOrderID",
                table: "RefundRequests",
                column: "RentalOrderID",
                principalTable: "RentalOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefundRequests_SaleOrders_SaleOrderID",
                table: "RefundRequests",
                column: "SaleOrderID",
                principalTable: "SaleOrders",
                principalColumn: "SaleOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
