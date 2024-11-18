using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_staffTable_and_managerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Staffs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Managers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RefundRequestRefundID",
                table: "ImagesVideos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefundRequests",
                columns: table => new
                {
                    RefundID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleOrderID = table.Column<int>(type: "int", nullable: false),
                    SaleOrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RentalOrderID = table.Column<int>(type: "int", nullable: false),
                    RentalOrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    RefundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefundMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentGatewayTransactionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<int>(type: "int", nullable: true),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAgreementAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequests", x => x.RefundID);
                    table.ForeignKey(
                        name: "FK_RefundRequests_RentalOrders_RentalOrderID",
                        column: x => x.RentalOrderID,
                        principalTable: "RentalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundRequests_SaleOrders_SaleOrderID",
                        column: x => x.SaleOrderID,
                        principalTable: "SaleOrders",
                        principalColumn: "SaleOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesVideos_RefundRequestRefundID",
                table: "ImagesVideos",
                column: "RefundRequestRefundID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_RentalOrderID",
                table: "RefundRequests",
                column: "RentalOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_SaleOrderID",
                table: "RefundRequests",
                column: "SaleOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagesVideos_RefundRequests_RefundRequestRefundID",
                table: "ImagesVideos",
                column: "RefundRequestRefundID",
                principalTable: "RefundRequests",
                principalColumn: "RefundID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagesVideos_RefundRequests_RefundRequestRefundID",
                table: "ImagesVideos");

            migrationBuilder.DropTable(
                name: "RefundRequests");

            migrationBuilder.DropIndex(
                name: "IX_ImagesVideos_RefundRequestRefundID",
                table: "ImagesVideos");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "RefundRequestRefundID",
                table: "ImagesVideos");
        }
    }
}
