using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.Migrations
{
    public partial class UpdateVote_Model_new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Areas_AreaID",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_AreaID",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "Votes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaID",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_AreaID",
                table: "Votes",
                column: "AreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Areas_AreaID",
                table: "Votes",
                column: "AreaID",
                principalTable: "Areas",
                principalColumn: "AreaID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
