using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modifyimporthistorymodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Users_UserId",
                table: "ImportHistories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ImportHistories",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_ImportHistories_UserId",
                table: "ImportHistories",
                newName: "IX_ImportHistories_EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Employees_EmployeeId",
                table: "ImportHistories",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Employees_EmployeeId",
                table: "ImportHistories");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "ImportHistories",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ImportHistories_EmployeeId",
                table: "ImportHistories",
                newName: "IX_ImportHistories_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Users_UserId",
                table: "ImportHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
