using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchlistAndViewingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockFear",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "BlockLanguage",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "BlockViolence",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "PrefersMovies",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "PrefersSeries",
                table: "ProfilePreferences");

            migrationBuilder.RenameColumn(
                name: "MinAgeAllowed",
                table: "ProfilePreferences",
                newName: "MinimumAge");

            migrationBuilder.RenameColumn(
                name: "GenrePreference",
                table: "ProfilePreferences",
                newName: "ContentType");

            migrationBuilder.AddColumn<List<string>>(
                name: "ContentFilters",
                table: "ProfilePreferences",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "PreferredGenres",
                table: "ProfilePreferences",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "WatchlistId",
                table: "Contents",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ViewingHistories",
                columns: table => new
                {
                    ViewingHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileId = table.Column<int>(type: "integer", nullable: false),
                    ContentId = table.Column<int>(type: "integer", nullable: false),
                    EpisodeId = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastPosition = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewingHistories", x => x.ViewingHistoryId);
                    table.ForeignKey(
                        name: "FK_ViewingHistories_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Watchlists",
                columns: table => new
                {
                    WatchlistId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileId = table.Column<int>(type: "integer", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchlists", x => x.WatchlistId);
                    table.ForeignKey(
                        name: "FK_Watchlists_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_WatchlistId",
                table: "Contents",
                column: "WatchlistId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingHistories_ProfileId",
                table: "ViewingHistories",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_ProfileId",
                table: "Watchlists",
                column: "ProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Watchlists_WatchlistId",
                table: "Contents",
                column: "WatchlistId",
                principalTable: "Watchlists",
                principalColumn: "WatchlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Watchlists_WatchlistId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "ViewingHistories");

            migrationBuilder.DropTable(
                name: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Contents_WatchlistId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ContentFilters",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "PreferredGenres",
                table: "ProfilePreferences");

            migrationBuilder.DropColumn(
                name: "WatchlistId",
                table: "Contents");

            migrationBuilder.RenameColumn(
                name: "MinimumAge",
                table: "ProfilePreferences",
                newName: "MinAgeAllowed");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "ProfilePreferences",
                newName: "GenrePreference");

            migrationBuilder.AddColumn<bool>(
                name: "BlockFear",
                table: "ProfilePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockLanguage",
                table: "ProfilePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BlockViolence",
                table: "ProfilePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrefersMovies",
                table: "ProfilePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrefersSeries",
                table: "ProfilePreferences",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
