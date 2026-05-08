using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safespace.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "usercode",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usercode",
                table: "User");
        }
    }
}
