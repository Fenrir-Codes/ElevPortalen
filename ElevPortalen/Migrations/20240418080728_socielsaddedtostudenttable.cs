using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations
{
    /// <inheritdoc />
    public partial class socielsaddedtostudenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookLink",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInLink",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramLink",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubLink",
                table: "Student",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
