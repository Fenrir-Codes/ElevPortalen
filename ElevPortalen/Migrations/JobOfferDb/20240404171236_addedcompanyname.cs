using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations.JobOfferDb
{
    /// <inheritdoc />
    public partial class addedcompanyname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "JobOfferDataBase",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "JobOfferDataBase");
        }
    }
}
