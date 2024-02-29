using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations.ElevPortalenDataDb
{
    /// <inheritdoc />
    public partial class changednameondatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skill_Student_StudentId",
                table: "Skill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skill",
                table: "Skill");

            migrationBuilder.RenameTable(
                name: "Skill",
                newName: "StudentSkills");

            migrationBuilder.RenameIndex(
                name: "IX_Skill_StudentId",
                table: "StudentSkills",
                newName: "IX_StudentSkills_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSkills",
                table: "StudentSkills",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSkills_Student_StudentId",
                table: "StudentSkills",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSkills_Student_StudentId",
                table: "StudentSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSkills",
                table: "StudentSkills");

            migrationBuilder.RenameTable(
                name: "StudentSkills",
                newName: "Skill");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSkills_StudentId",
                table: "Skill",
                newName: "IX_Skill_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skill",
                table: "Skill",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skill_Student_StudentId",
                table: "Skill",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
