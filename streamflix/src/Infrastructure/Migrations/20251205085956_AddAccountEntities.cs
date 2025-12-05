using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "ContentContentWarnings");

            migrationBuilder.DropTable(
                name: "ContentQualities");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "ContentWarnings");

            migrationBuilder.DropTable(
                name: "Qualities");

            migrationBuilder.DropIndex(
                name: "IX_Contents_GenreId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Contents");

            migrationBuilder.AddColumn<List<string>>(
                name: "AvailableQualities",
                table: "Contents",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "ContentWarnings",
                table: "Contents",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Contents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    BlockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerificationToken = table.Column<string>(type: "text", nullable: true),
                    TokenExpire = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    ReferralId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accountId = table.Column<int>(type: "integer", nullable: false),
                    subscriptionType = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<double>(type: "double precision", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsTrialPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    TrialPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.ReferralId);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionType = table.Column<string>(type: "text", nullable: false),
                    SubscriptionDescription = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<double>(type: "double precision", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsTrialPeriod = table.Column<bool>(type: "boolean", nullable: false),
                    TrialPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.SubscriptionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "AvailableQualities",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ContentWarnings",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Contents");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Contents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
