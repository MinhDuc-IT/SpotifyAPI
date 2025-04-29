using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateSearchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Query",
                table: "SearchHistories",
                newName: "ImageURL");

            migrationBuilder.AddColumn<string>(
                name: "AudioURL",
                table: "SearchHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SearchHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ResultID",
                table: "SearchHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SearchHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioURL",
                table: "SearchHistories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SearchHistories");

            migrationBuilder.DropColumn(
                name: "ResultID",
                table: "SearchHistories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SearchHistories");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "SearchHistories",
                newName: "Query");
        }
    }
}
