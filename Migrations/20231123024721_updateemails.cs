using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESPRESSO.Migrations
{
    /// <inheritdoc />
    public partial class updateemails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "emailId",
                table: "pagecounter",
                newName: "EMAIL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMAIL",
                table: "pagecounter",
                newName: "emailId");
        }
    }
}
