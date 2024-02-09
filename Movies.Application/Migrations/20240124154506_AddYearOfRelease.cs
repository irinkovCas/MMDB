using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddYearOfRelease : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearOfRelease",
                table: "Movies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearOfRelease",
                table: "Movies");
        }
    }
}
