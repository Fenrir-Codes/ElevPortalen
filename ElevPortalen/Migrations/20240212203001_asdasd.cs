using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations
{
    /// <inheritdoc />
    public partial class asdasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HRecoveryCreatedDate",
                table: "CompanyDataRecovery",
                newName: "RecoveryCreatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecoveryCreatedDate",
                table: "CompanyDataRecovery",
                newName: "HRecoveryCreatedDate");
        }
    }
}
