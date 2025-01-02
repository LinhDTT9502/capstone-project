using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modifyforeignkeyinblogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_CreatedStaffId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_EditedByStaffId",
                table: "Blogs");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_CreatedStaffId",
                table: "Blogs",
                column: "CreatedStaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_EditedByStaffId",
                table: "Blogs",
                column: "EditedByStaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_CreatedStaffId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_EditedByStaffId",
                table: "Blogs");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Staffs_CreatedStaffId",
                table: "Blogs",
                column: "CreatedStaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Staffs_EditedByStaffId",
                table: "Blogs",
                column: "EditedByStaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
