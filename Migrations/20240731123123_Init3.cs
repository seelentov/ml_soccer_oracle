using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeadToHeadWinsTeam2",
                table: "HeadToHeadStats",
                newName: "WinsTeam2");

            migrationBuilder.RenameColumn(
                name: "HeadToHeadWinsTeam1",
                table: "HeadToHeadStats",
                newName: "WinsTeam1");

            migrationBuilder.RenameColumn(
                name: "HeadToHeadDraws",
                table: "HeadToHeadStats",
                newName: "Draws");

            migrationBuilder.RenameColumn(
                name: "AverageGoalsInHeadToHead",
                table: "HeadToHeadStats",
                newName: "AverageGoals");

            migrationBuilder.RenameColumn(
                name: "AverageGoalDifferenceInHeadToHead",
                table: "HeadToHeadStats",
                newName: "AverageGoalDifference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WinsTeam2",
                table: "HeadToHeadStats",
                newName: "HeadToHeadWinsTeam2");

            migrationBuilder.RenameColumn(
                name: "WinsTeam1",
                table: "HeadToHeadStats",
                newName: "HeadToHeadWinsTeam1");

            migrationBuilder.RenameColumn(
                name: "Draws",
                table: "HeadToHeadStats",
                newName: "HeadToHeadDraws");

            migrationBuilder.RenameColumn(
                name: "AverageGoals",
                table: "HeadToHeadStats",
                newName: "AverageGoalsInHeadToHead");

            migrationBuilder.RenameColumn(
                name: "AverageGoalDifference",
                table: "HeadToHeadStats",
                newName: "AverageGoalDifferenceInHeadToHead");
        }
    }
}
