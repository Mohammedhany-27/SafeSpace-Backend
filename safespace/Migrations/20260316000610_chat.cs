using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safespace.Migrations
{
    /// <inheritdoc />
    public partial class chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Chat_ChatId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Doctor_DoctorProfileId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "DoctorProfileId",
                table: "Review",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_DoctorProfileId",
                table: "Review",
                newName: "IX_Review_DoctorId");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Message",
                newName: "senderId");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Message",
                newName: "chatId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Message",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Message",
                newName: "sendAt");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ChatId",
                table: "Message",
                newName: "IX_Message_chatId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Chat",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Chat",
                newName: "id");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Chat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Chat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatientProfileId",
                table: "Chat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chat_DoctorId",
                table: "Chat",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_PatientProfileId",
                table: "Chat",
                column: "PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Doctor_DoctorId",
                table: "Chat",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_PatientProfile_PatientProfileId",
                table: "Chat",
                column: "PatientProfileId",
                principalTable: "PatientProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_chatId",
                table: "Message",
                column: "chatId",
                principalTable: "Chat",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Doctor_DoctorId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_PatientProfile_PatientProfileId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Chat_chatId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Chat_DoctorId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_PatientProfileId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "PatientProfileId",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Review",
                newName: "DoctorProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_DoctorId",
                table: "Review",
                newName: "IX_Review_DoctorProfileId");

            migrationBuilder.RenameColumn(
                name: "senderId",
                table: "Message",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "chatId",
                table: "Message",
                newName: "ChatId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Message",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sendAt",
                table: "Message",
                newName: "SentAt");

            migrationBuilder.RenameIndex(
                name: "IX_Message_chatId",
                table: "Message",
                newName: "IX_Message_ChatId");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Chat",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Chat",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Chat",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Chat",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorProfileId = table.Column<int>(type: "int", nullable: false),
                    PatientProfileId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctor_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_PatientProfile_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorProfileId",
                table: "Appointments",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientProfileId",
                table: "Appointments",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_ChatId",
                table: "ChatParticipants",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_UserId",
                table: "ChatParticipants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_ChatId",
                table: "Message",
                column: "ChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Doctor_DoctorProfileId",
                table: "Review",
                column: "DoctorProfileId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
