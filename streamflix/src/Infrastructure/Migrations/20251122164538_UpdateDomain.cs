using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Movies");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "Contents");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Contents",
                newName: "ContentId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Contents",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgeRating",
                table: "Contents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Contents",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Contents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Contents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalSeasons",
                table: "Contents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contents",
                table: "Contents",
                column: "ContentId");

            migrationBuilder.CreateTable(
                name: "ContentWarnings",
                columns: table => new
                {
                    ContentWarningId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentWarningType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentWarnings", x => x.ContentWarningId);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GenreType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.GenreId);
                });

            migrationBuilder.CreateTable(
                name: "Qualities",
                columns: table => new
                {
                    QualityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QualityType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualities", x => x.QualityId);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SeasonNumber = table.Column<int>(type: "integer", nullable: false),
                    TotalEpisodes = table.Column<int>(type: "integer", nullable: false),
                    SeriesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.SeasonId);
                    table.ForeignKey(
                        name: "FK_Seasons_Contents_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentContentWarnings",
                columns: table => new
                {
                    ContentWarningsContentWarningId = table.Column<int>(type: "integer", nullable: false),
                    ContentsContentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentContentWarnings", x => new { x.ContentWarningsContentWarningId, x.ContentsContentId });
                    table.ForeignKey(
                        name: "FK_ContentContentWarnings_ContentWarnings_ContentWarningsConte~",
                        column: x => x.ContentWarningsContentWarningId,
                        principalTable: "ContentWarnings",
                        principalColumn: "ContentWarningId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentContentWarnings_Contents_ContentsContentId",
                        column: x => x.ContentsContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentQualities",
                columns: table => new
                {
                    AvailableQualitiesQualityId = table.Column<int>(type: "integer", nullable: false),
                    ContentsContentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentQualities", x => new { x.AvailableQualitiesQualityId, x.ContentsContentId });
                    table.ForeignKey(
                        name: "FK_ContentQualities_Contents_ContentsContentId",
                        column: x => x.ContentsContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentQualities_Qualities_AvailableQualitiesQualityId",
                        column: x => x.AvailableQualitiesQualityId,
                        principalTable: "Qualities",
                        principalColumn: "QualityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    EpisodeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EpisodeNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    SeasonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.EpisodeId);
                    table.ForeignKey(
                        name: "FK_Episodes_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_GenreId",
                table: "Contents",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentContentWarnings_ContentsContentId",
                table: "ContentContentWarnings",
                column: "ContentsContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentQualities_ContentsContentId",
                table: "ContentQualities",
                column: "ContentsContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SeasonId",
                table: "Episodes",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeriesId",
                table: "Seasons",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "ContentContentWarnings");

            migrationBuilder.DropTable(
                name: "ContentQualities");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "ContentWarnings");

            migrationBuilder.DropTable(
                name: "Qualities");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contents",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_GenreId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "AgeRating",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "TotalSeasons",
                table: "Contents");

            migrationBuilder.RenameTable(
                name: "Contents",
                newName: "Movies");

            migrationBuilder.RenameColumn(
                name: "ContentId",
                table: "Movies",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Movies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Movies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Movies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "Id");
        }
    }
}
