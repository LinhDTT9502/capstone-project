using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class add_returnRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    ReturnID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleOrderID = table.Column<int>(type: "int", nullable: true),
                    RentalOrderID = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition = table.Column<int>(type: "int", nullable: true),
                    ReturnAmount = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ProcessedBy = table.Column<int>(type: "int", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.ReturnID);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_RentalOrders_RentalOrderID",
                        column: x => x.RentalOrderID,
                        principalTable: "RentalOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReturnRequests_SaleOrders_SaleOrderID",
                        column: x => x.SaleOrderID,
                        principalTable: "SaleOrders",
                        principalColumn: "SaleOrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_RentalOrderID",
                table: "ReturnRequests",
                column: "RentalOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_SaleOrderID",
                table: "ReturnRequests",
                column: "SaleOrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnRequests");
        }
    }
}
