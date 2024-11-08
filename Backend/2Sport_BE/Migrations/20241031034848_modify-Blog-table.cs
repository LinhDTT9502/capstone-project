using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modifyBlogtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_UserId",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_UserId",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Blogs",
                newName: "CreatedStaffId");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByStaffStaffId",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EditedByStaffId",
                table: "Blogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Blogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Blogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedByStaffStaffId",
                table: "Blogs",
                column: "CreatedByStaffStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_EditedByStaffId",
                table: "Blogs",
                column: "EditedByStaffId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Blogs_EditedByStaffId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedByStaffStaffId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "EditedByStaffId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "CreatedStaffId",
                table: "Blogs",
                newName: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UserId",
                table: "Blogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_UserId",
                table: "Blogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
