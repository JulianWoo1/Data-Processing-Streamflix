using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ViewingHistoryManyToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ViewingHistories_ProfileId",
                table: "ViewingHistories");

            migrationBuilder.DropColumn(
                name: "ProfilePreferenceId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "ViewingHistoryId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "Profiles");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingHistories_ProfileId",
                table: "ViewingHistories",
                column: "ProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ViewingHistories_ProfileId",
                table: "ViewingHistories");

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

            migrationBuilder.CreateIndex(
                name: "IX_ViewingHistories_ProfileId",
                table: "ViewingHistories",
                column: "ProfileId",
                unique: true);
        }
    }
}
