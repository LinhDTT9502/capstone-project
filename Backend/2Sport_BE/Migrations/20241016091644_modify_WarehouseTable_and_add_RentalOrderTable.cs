using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_WarehouseTable_and_add_RentalOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Warehouses",
                newName: "TotalQuantity");

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "Warehouses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RentalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    RentalStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RentalEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LateFee = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    IsRestocked = table.Column<bool>(type: "bit", nullable: false),
                    IsInspected = table.Column<bool>(type: "bit", nullable: false),
                    DamageFee = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_RentalOrders_OrderId",
                table: "RentalOrders",
                column: "OrderId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Branches_BranchId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Guests_GuestId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "RentalOrders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BranchId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_GuestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TotalQuantity",
                table: "Warehouses",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderCode",
                table: "Orders",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
