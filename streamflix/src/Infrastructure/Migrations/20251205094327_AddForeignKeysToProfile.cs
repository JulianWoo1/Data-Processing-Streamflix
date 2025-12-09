using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfilePreferenceId",
                table: "Profiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewingHistoryId",
                table: "Profiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WatchlistId",
                table: "Profiles",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePreferenceId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ViewingHistoryId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "Profiles");
        }
    }
}
