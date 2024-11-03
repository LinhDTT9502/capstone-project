using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class updateImportHistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotCode",
                table: "ImportHistories");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ImportHistories",
                newName: "Action");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "ImportHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ImportHistories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Condition",
                table: "ImportHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "ImportHistories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "ImportHistories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ImportHistories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "ImportHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ImportHistories_StaffId",
                table: "ImportHistories",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories",
                column: "StaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportHistories_Staffs_StaffId",
                table: "ImportHistories");

            migrationBuilder.DropIndex(
                name: "IX_ImportHistories_StaffId",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "ImportHistories");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "ImportHistories");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "ImportHistories",
                newName: "Content");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "ImportHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotCode",
                table: "ImportHistories",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
