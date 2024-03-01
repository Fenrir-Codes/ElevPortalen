using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations.ElevPortalenDataDb
{
    /// <inheritdoc />
    public partial class aaassdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentSkills_StudentId",
                table: "StudentSkills");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_StudentId",
                table: "StudentSkills",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentSkills_StudentId",
                table: "StudentSkills");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_StudentId",
                table: "StudentSkills",
                column: "StudentId");
        }
    }
}
