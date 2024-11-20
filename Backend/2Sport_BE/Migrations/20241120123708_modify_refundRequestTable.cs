using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modify_refundRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagesVideos_RefundRequests_RefundRequestRefundID",
                table: "ImagesVideos");

            migrationBuilder.DropIndex(
                name: "IX_ImagesVideos_RefundRequestRefundID",
                table: "ImagesVideos");

            migrationBuilder.DropColumn(
                name: "RefundRequestRefundID",
                table: "ImagesVideos");

            migrationBuilder.RenameColumn(
                name: "RefundDate",
                table: "RefundRequests",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "StaffName",
                table: "RefundRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "StaffNotes",
                table: "RefundRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RefundRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffNotes",
                table: "RefundRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RefundRequests");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "RefundRequests",
                newName: "RefundDate");

            migrationBuilder.AlterColumn<string>(
                name: "StaffName",
                table: "RefundRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundRequestRefundID",
                table: "ImagesVideos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagesVideos_RefundRequestRefundID",
                table: "ImagesVideos",
                column: "RefundRequestRefundID");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagesVideos_RefundRequests_RefundRequestRefundID",
                table: "ImagesVideos",
                column: "RefundRequestRefundID",
                principalTable: "RefundRequests",
                principalColumn: "RefundID");
        }
    }
}
