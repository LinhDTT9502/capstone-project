using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDetails_SupervisorId",
                table: "EmployeeDetails",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDetails_Employees_SupervisorId",
                table: "EmployeeDetails",
                column: "SupervisorId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDetails_Employees_SupervisorId",
                table: "EmployeeDetails");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeDetails_SupervisorId",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "EmployeeDetails");
        }

    }
}
