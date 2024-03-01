using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations.ElevPortalenDataDb
{
    /// <inheritdoc />
    public partial class aaaaaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OOP",
                table: "StudentSkills",
                newName: "WillingToLearn");

            migrationBuilder.RenameColumn(
                name: "MongoDB",
                table: "StudentSkills",
                newName: "TeamWorking");

            migrationBuilder.AddColumn<bool>(
                name: "Communikation",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NetWork",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProblemSolving",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Communikation",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "NetWork",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "ProblemSolving",
                table: "StudentSkills");

            migrationBuilder.RenameColumn(
                name: "WillingToLearn",
                table: "StudentSkills",
                newName: "OOP");

            migrationBuilder.RenameColumn(
                name: "TeamWorking",
                table: "StudentSkills",
                newName: "MongoDB");
        }
    }
}
