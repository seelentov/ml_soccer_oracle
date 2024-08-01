using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayingTime",
                table: "StandardStats");

            migrationBuilder.DropColumn(
                name: "IsHomeMatchForTeam1",
                table: "Games");

            migrationBuilder.AddColumn<bool>(
                name: "Parsed",
                table: "Leagues",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parsed",
                table: "Leagues");

            migrationBuilder.AddColumn<float>(
                name: "PlayingTime",
                table: "StandardStats",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "IsHomeMatchForTeam1",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
