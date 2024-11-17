using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.Migrations
{
    public partial class UpdateCandidate_Area : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AreaName",
                table: "Areas",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Areas",
                newName: "AreaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Areas",
                newName: "AreaName");

            migrationBuilder.RenameColumn(
                name: "AreaID",
                table: "Areas",
                newName: "Id");
        }
    }
}
