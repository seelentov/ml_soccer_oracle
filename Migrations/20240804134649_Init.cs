using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeadToHeadBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Matches = table.Column<float>(type: "REAL", nullable: false),
                    RestHours = table.Column<float>(type: "REAL", nullable: false),
                    Wins = table.Column<float>(type: "REAL", nullable: false),
                    Draws = table.Column<float>(type: "REAL", nullable: false),
                    Loses = table.Column<float>(type: "REAL", nullable: false),
                    Goals = table.Column<float>(type: "REAL", nullable: false),
                    GoalsLost = table.Column<float>(type: "REAL", nullable: false),
                    GoalsGame = table.Column<float>(type: "REAL", nullable: false),
                    GoalsGameLost = table.Column<float>(type: "REAL", nullable: false),
                    DryMatches = table.Column<float>(type: "REAL", nullable: false),
                    GoalBoth = table.Column<float>(type: "REAL", nullable: false),
                    TotalMore25 = table.Column<float>(type: "REAL", nullable: false),
                    TotalLess25 = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadToHeadBase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeadToHeadInGame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Strikes = table.Column<float>(type: "REAL", nullable: false),
                    ShotsOnTarget = table.Column<float>(type: "REAL", nullable: false),
                    Possession = table.Column<float>(type: "REAL", nullable: false),
                    Corner = table.Column<float>(type: "REAL", nullable: false),
                    Violations = table.Column<float>(type: "REAL", nullable: false),
                    Offsides = table.Column<float>(type: "REAL", nullable: false),
                    RedCards = table.Column<float>(type: "REAL", nullable: false),
                    YellowCards = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadToHeadInGame", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ParseType = table.Column<int>(type: "INTEGER", nullable: false),
                    Parsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    HeadToHeadBaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    HeadToHeadInGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    HeadToHeadInGameOpponentId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_HeadToHeadBase_HeadToHeadBaseId",
                        column: x => x.HeadToHeadBaseId,
                        principalTable: "HeadToHeadBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_HeadToHeadInGame_HeadToHeadInGameId",
                        column: x => x.HeadToHeadInGameId,
                        principalTable: "HeadToHeadInGame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_HeadToHeadInGame_HeadToHeadInGameOpponentId",
                        column: x => x.HeadToHeadInGameOpponentId,
                        principalTable: "HeadToHeadInGame",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueId = table.Column<int>(type: "INTEGER", nullable: false),
                    Team1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Team2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Result = table.Column<float>(type: "REAL", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_Team1Id",
                        column: x => x.Team1Id,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_Team2Id",
                        column: x => x.Team2Id,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_LeagueId",
                table: "Games",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Team1Id",
                table: "Games",
                column: "Team1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Team2Id",
                table: "Games",
                column: "Team2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_HeadToHeadBaseId",
                table: "Teams",
                column: "HeadToHeadBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_HeadToHeadInGameId",
                table: "Teams",
                column: "HeadToHeadInGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_HeadToHeadInGameOpponentId",
                table: "Teams",
                column: "HeadToHeadInGameOpponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "HeadToHeadBase");

            migrationBuilder.DropTable(
                name: "HeadToHeadInGame");
        }
    }
}
