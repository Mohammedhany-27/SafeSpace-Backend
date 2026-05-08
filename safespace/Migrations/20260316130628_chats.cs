using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safespace.Migrations
{
    /// <inheritdoc />
    public partial class chats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Chat",
                newName: "LastMessageTime");

            migrationBuilder.AddColumn<bool>(
                name: "isRead",
                table: "Message",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRead",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "LastMessageTime",
                table: "Chat",
                newName: "createdAt");
        }
    }
}
