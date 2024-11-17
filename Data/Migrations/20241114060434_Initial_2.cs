using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.Migrations
{
    public partial class Initial_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Areas_AreaId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Candidates",
                newName: "AreaID");

            migrationBuilder.RenameIndex(
                name: "IX_Candidates_AreaId",
                table: "Candidates",
                newName: "IX_Candidates_AreaID");

            migrationBuilder.AlterColumn<int>(
                name: "AreaID",
                table: "Candidates",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Areas_AreaID",
                table: "Candidates",
                column: "AreaID",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Areas_AreaID",
                table: "Candidates");

            migrationBuilder.RenameColumn(
                name: "AreaID",
                table: "Candidates",
                newName: "AreaId");

            migrationBuilder.RenameIndex(
                name: "IX_Candidates_AreaID",
                table: "Candidates",
                newName: "IX_Candidates_AreaId");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Candidates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Candidates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Areas_AreaId",
                table: "Candidates",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id");
        }
    }
}
