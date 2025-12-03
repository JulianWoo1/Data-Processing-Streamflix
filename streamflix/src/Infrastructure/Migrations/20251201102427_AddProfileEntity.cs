using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AgeCategory = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "ProfilePreferences",
                columns: table => new
                {
                    ProfilePreferenceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileId = table.Column<int>(type: "integer", nullable: false),
                    GenrePreference = table.Column<string>(type: "text", nullable: false),
                    PrefersMovies = table.Column<bool>(type: "boolean", nullable: false),
                    PrefersSeries = table.Column<bool>(type: "boolean", nullable: false),
                    MinAgeAllowed = table.Column<int>(type: "integer", nullable: false),
                    BlockViolence = table.Column<bool>(type: "boolean", nullable: false),
                    BlockFear = table.Column<bool>(type: "boolean", nullable: false),
                    BlockLanguage = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePreferences", x => x.ProfilePreferenceId);
                    table.ForeignKey(
                        name: "FK_ProfilePreferences_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePreferences_ProfileId",
                table: "ProfilePreferences",
                column: "ProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePreferences");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
