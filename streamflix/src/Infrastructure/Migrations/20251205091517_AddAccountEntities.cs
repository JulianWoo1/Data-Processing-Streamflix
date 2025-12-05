using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_ProfileId",
                table: "Watchlists",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_Profiles_ProfileId",
                table: "Watchlists",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_Profiles_ProfileId",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_ProfileId",
                table: "Watchlists");
        }
    }
}
