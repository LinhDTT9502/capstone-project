using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class addmodifycolumnsinImportHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "ImportHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "RentPrice",
                table: "ImportHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ImportHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "ImportHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImportHistories_ManagerId",
                table: "ImportHistories",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Managers_ManagerId",
                table: "ImportHistories",
                column: "ManagerId",
                principalTable: "Managers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Managers_ManagerId",
                table: "ImportHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories");

            migrationBuilder.DropIndex(
                name: "IX_ImportHistories_ManagerId",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "ImportHistories");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "ImportHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RentPrice",
                table: "ImportHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "ImportHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
