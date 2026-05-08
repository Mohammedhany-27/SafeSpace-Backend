using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safespace.Migrations
{
    /// <inheritdoc />
    public partial class FixUserInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_User_PatientProfileId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "PatientProfileId",
                table: "Sessions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_PatientProfileId",
                table: "Sessions",
                newName: "IX_Sessions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_User_UserId",
                table: "Sessions",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_User_UserId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Sessions",
                newName: "PatientProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                newName: "IX_Sessions_PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_User_PatientProfileId",
                table: "Sessions",
                column: "PatientProfileId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
