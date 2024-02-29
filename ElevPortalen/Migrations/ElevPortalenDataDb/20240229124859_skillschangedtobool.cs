using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations.ElevPortalenDataDb
{
    /// <inheritdoc />
    public partial class skillschangedtobool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillName",
                table: "StudentSkills");

            migrationBuilder.AddColumn<bool>(
                name: "Blazor",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Bootstrap",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "C",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CPlusPlus",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CSS",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CSharp",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CloudComputing",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DotNet",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HTML",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Java",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "JavaScript",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MongoDB",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OOP",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OfficePack",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PHP",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Python",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SQL",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Typescript",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VersionControl",
                table: "StudentSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blazor",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "Bootstrap",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "C",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "CPlusPlus",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "CSS",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "CSharp",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "CloudComputing",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "DotNet",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "HTML",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "Java",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "JavaScript",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "MongoDB",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "OOP",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "OfficePack",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "PHP",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "Python",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "SQL",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "Typescript",
                table: "StudentSkills");

            migrationBuilder.DropColumn(
                name: "VersionControl",
                table: "StudentSkills");

            migrationBuilder.AddColumn<string>(
                name: "SkillName",
                table: "StudentSkills",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
