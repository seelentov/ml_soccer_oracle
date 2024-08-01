using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeadToHeadStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeadToHeadWinsTeam1 = table.Column<float>(type: "REAL", nullable: false),
                    HeadToHeadWinsTeam2 = table.Column<float>(type: "REAL", nullable: false),
                    HeadToHeadDraws = table.Column<float>(type: "REAL", nullable: false),
                    AverageGoalsInHeadToHead = table.Column<float>(type: "REAL", nullable: false),
                    AverageGoalDifferenceInHeadToHead = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadToHeadStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MinStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchesPlayed = table.Column<float>(type: "REAL", nullable: false),
                    Wins = table.Column<float>(type: "REAL", nullable: false),
                    Draws = table.Column<float>(type: "REAL", nullable: false),
                    Loses = table.Column<float>(type: "REAL", nullable: false),
                    GoalsFor = table.Column<float>(type: "REAL", nullable: false),
                    GoalsAgainst = table.Column<float>(type: "REAL", nullable: false),
                    GoalsDifference = table.Column<float>(type: "REAL", nullable: false),
                    Points = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NineteenStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Minutes = table.Column<float>(type: "REAL", nullable: false),
                    Goals = table.Column<float>(type: "REAL", nullable: false),
                    Assists = table.Column<float>(type: "REAL", nullable: false),
                    NonPenaltyGoals = table.Column<float>(type: "REAL", nullable: false),
                    PenaltyAssists = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NineteenStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MatchesPlayed = table.Column<float>(type: "REAL", nullable: false),
                    Starts = table.Column<float>(type: "REAL", nullable: false),
                    Minutes = table.Column<float>(type: "REAL", nullable: false),
                    PlayingTime = table.Column<float>(type: "REAL", nullable: false),
                    Goals = table.Column<float>(type: "REAL", nullable: false),
                    Assists = table.Column<float>(type: "REAL", nullable: false),
                    NonPenaltyGoals = table.Column<float>(type: "REAL", nullable: false),
                    PenaltyMade = table.Column<float>(type: "REAL", nullable: false),
                    PenaltyAttempted = table.Column<float>(type: "REAL", nullable: false),
                    YellowCards = table.Column<float>(type: "REAL", nullable: false),
                    RedCards = table.Column<float>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<float>(type: "REAL", nullable: false),
                    StandardStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    NineteenStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_NineteenStats_NineteenStatsId",
                        column: x => x.NineteenStatsId,
                        principalTable: "NineteenStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Players_StandardStats_StandardStatsId",
                        column: x => x.StandardStatsId,
                        principalTable: "StandardStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MinStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TopTeamScorerId = table.Column<int>(type: "INTEGER", nullable: false),
                    GoalkeeperId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_MinStats_MinStatsId",
                        column: x => x.MinStatsId,
                        principalTable: "MinStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_Players_GoalkeeperId",
                        column: x => x.GoalkeeperId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_Players_TopTeamScorerId",
                        column: x => x.TopTeamScorerId,
                        principalTable: "Players",
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
                    IsHomeMatchForTeam1 = table.Column<bool>(type: "INTEGER", nullable: false),
                    HeadToHeadStatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Result = table.Column<float>(type: "REAL", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_HeadToHeadStats_HeadToHeadStatsId",
                        column: x => x.HeadToHeadStatsId,
                        principalTable: "HeadToHeadStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_Games_HeadToHeadStatsId",
                table: "Games",
                column: "HeadToHeadStatsId");

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
                name: "IX_Players_NineteenStatsId",
                table: "Players",
                column: "NineteenStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_StandardStatsId",
                table: "Players",
                column: "StandardStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_GoalkeeperId",
                table: "Teams",
                column: "GoalkeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_MinStatsId",
                table: "Teams",
                column: "MinStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TopTeamScorerId",
                table: "Teams",
                column: "TopTeamScorerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "HeadToHeadStats");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "MinStats");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "NineteenStats");

            migrationBuilder.DropTable(
                name: "StandardStats");
        }
    }
}
