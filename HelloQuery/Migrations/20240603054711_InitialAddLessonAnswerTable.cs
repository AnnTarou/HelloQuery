using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloQuery.Migrations
{
    /// <inheritdoc />
    public partial class InitialAddLessonAnswerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonAnswer",
                columns: table => new
                {
                    LessonAnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    ValidAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAnswer", x => x.LessonAnswerId);
                    table.ForeignKey(
                        name: "FK_LessonAnswer_Lesson_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lesson",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonAnswer_LessonId",
                table: "LessonAnswer",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonAnswer");
        }
    }
}
