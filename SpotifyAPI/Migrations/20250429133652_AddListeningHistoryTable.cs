using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpotifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddListeningHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListeningHistories",
                table: "ListeningHistories");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ListeningHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListeningHistories",
                table: "ListeningHistories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistories_UserID",
                table: "ListeningHistories",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListeningHistories",
                table: "ListeningHistories");

            migrationBuilder.DropIndex(
                name: "IX_ListeningHistories_UserID",
                table: "ListeningHistories");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ListeningHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListeningHistories",
                table: "ListeningHistories",
                columns: new[] { "UserID", "SongId" });
        }
    }
}
