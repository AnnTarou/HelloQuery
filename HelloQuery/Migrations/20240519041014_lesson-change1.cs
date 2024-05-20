using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloQuery.Migrations
{
    /// <inheritdoc />
    public partial class lessonchange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Categry",
                table: "Lesson",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Lesson",
                newName: "Categry");
        }
    }
}
