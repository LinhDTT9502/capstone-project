using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Migrations
{
    /// <inheritdoc />
    public partial class removeBrandBranchandBrandCategorytables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandBranches");

            migrationBuilder.DropTable(
                name: "BrandCategories");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    BrandId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BrandBranches_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BrandCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandCategories_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BrandCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandBranches_BranchId",
                table: "BrandBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandBranches_BrandId",
                table: "BrandBranches",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCategories_BrandId",
                table: "BrandCategories",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandCategories_CategoryId",
                table: "BrandCategories",
                column: "CategoryId");
        }
    }
}
