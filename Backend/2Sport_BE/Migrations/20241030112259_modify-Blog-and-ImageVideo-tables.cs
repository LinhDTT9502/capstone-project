using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class modifyBlogandImageVideotables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImagesVideos_Blogs_BlogId",
                table: "ImagesVideos");

            migrationBuilder.DropIndex(
                name: "IX_ImagesVideos_BlogId",
                table: "ImagesVideos");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "ImagesVideos");

            migrationBuilder.RenameColumn(
                name: "BlogName",
                table: "Blogs",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Blogs",
                newName: "BlogName");

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "ImagesVideos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagesVideos_BlogId",
                table: "ImagesVideos",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImagesVideos_Blogs_BlogId",
                table: "ImagesVideos",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }
    }
}
