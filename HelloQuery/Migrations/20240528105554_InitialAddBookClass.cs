using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloQuery.Migrations
{
    /// <inheritdoc />
    public partial class InitialAddBookClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    book_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "NVARCHAR(25)", nullable: false),
                    author = table.Column<string>(type: "NVARCHAR(10)", nullable: false),
                    publication_date = table.Column<DateTime>(type: "DATE", nullable: false),
                    pages = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.book_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books");
        }
    }
}
