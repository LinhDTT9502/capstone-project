using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefreshTokenTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột EmployeeId vào bảng RefreshTokens
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "RefreshTokens",
                type: "int",
                nullable: true);

            // Sau đó tạo chỉ mục
            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_EmployeeId",
                table: "RefreshTokens",
                column: "EmployeeId");

            // Tạo khóa ngoại nếu cần
            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Employees_EmployeeId",
                table: "RefreshTokens",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa chỉ mục
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_EmployeeId",
                table: "RefreshTokens");

            // Xóa khóa ngoại
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Employees_EmployeeId",
                table: "RefreshTokens");

            // Xóa cột EmployeeId
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "RefreshTokens");
        }

    }
}
