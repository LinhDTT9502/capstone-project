using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2Sport_BE.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSalaryHistoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "SalaryHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "SalaryHistories",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("SqlServer:Identity", "1, 1"),
                   UserId = table.Column<int>(type: "int", nullable: true),
                   EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                   Salary = table.Column<decimal>(type: "decimal(18,0)", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_SalaryHistories", x => x.Id);
                   table.ForeignKey(
                       name: "FK_SalaryHistories_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "Id");
               });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistories_UserId",
                table: "SalaryHistories",
                column: "UserId");
        }
    }
}
