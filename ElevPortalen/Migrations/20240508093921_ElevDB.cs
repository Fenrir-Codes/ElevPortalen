using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElevPortalen.Migrations
{
    /// <inheritdoc />
    public partial class ElevDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
     name: "Messages",
     columns: table => new
     {
         MessageId = table.Column<int>(type: "int", nullable: false)
             .Annotation("SqlServer:Identity", "1, 1"),
         ReceiverId = table.Column<int>(type: "int", nullable: false),
         SenderId = table.Column<int>(type: "int", nullable: false),
         SenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
         Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
         Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
         Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
         IsRead = table.Column<bool>(type: "bit", nullable: false)
     },
     constraints: table =>
     {
         table.PrimaryKey("PK_Messages", x => x.MessageId);
     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

        }
    }
}
