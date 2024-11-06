using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modifyCreatedStaffIdcolumninBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_CreatedByStaffStaffId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_EditedByStaffId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CreatedByStaffStaffId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedByStaffStaffId",
                table: "Blogs");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedStaffId",
                table: "Blogs",
                column: "CreatedStaffId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_CreatedStaffId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Staffs_EditedByStaffId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CreatedStaffId",
                table: "Blogs");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByStaffStaffId",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedByStaffStaffId",
                table: "Blogs",
                column: "CreatedByStaffStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Staffs_CreatedByStaffStaffId",
                table: "Blogs",
                column: "CreatedByStaffStaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Staffs_EditedByStaffId",
                table: "Blogs",
                column: "EditedByStaffId",
                principalTable: "Staffs",
                principalColumn: "StaffId");
        }
    }
}
