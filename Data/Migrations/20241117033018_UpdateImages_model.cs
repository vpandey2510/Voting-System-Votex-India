using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.Migrations
{
    public partial class UpdateImages_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoterImagePath",
                table: "Voters",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ElectionImagePath",
                table: "Elections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CandidateImagePath",
                table: "Candidates",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaImagePath",
                table: "Areas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoterImagePath",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "ElectionImagePath",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "CandidateImagePath",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "AreaImagePath",
                table: "Areas");
        }
    }
}
