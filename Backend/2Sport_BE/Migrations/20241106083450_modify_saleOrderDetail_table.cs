using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_saleOrderDetail_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "OrderDetails");
        }
    }
}
