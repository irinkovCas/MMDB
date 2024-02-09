using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Application.Migrations
{
    /// <inheritdoc />
    public partial class IgnoreRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Movies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Movies",
                type: "real",
                nullable: true);
        }
    }
}
