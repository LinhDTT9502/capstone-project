using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RenameStaffIdToEmployeeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "EmployeeDetails",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeDetails_StaffId",
                table: "EmployeeDetails",
                newName: "IX_EmployeeDetails_EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "EmployeeDetails",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeDetails_EmployeeId",
                table: "EmployeeDetails",
                newName: "IX_EmployeeDetails_StaffId");
        }

    }
}
