using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.Migrations
{
    public partial class UpdateVoter_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Voters");

            migrationBuilder.AddColumn<int>(
                name: "AreaID",
                table: "Voters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Voters_AreaID",
                table: "Voters",
                column: "AreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_Voters_Areas_AreaID",
                table: "Voters",
                column: "AreaID",
                principalTable: "Areas",
                principalColumn: "AreaID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voters_Areas_AreaID",
                table: "Voters");

            migrationBuilder.DropIndex(
                name: "IX_Voters_AreaID",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "AreaID",
                table: "Voters");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Voters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
