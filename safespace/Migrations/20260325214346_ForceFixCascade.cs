using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace safespace.Migrations
{
    /// <inheritdoc />
    public partial class ForceFixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailableSlots_Doctor_DoctorId",
                table: "AvailableSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_Certification_Doctor_DoctorId",
                table: "Certification");

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
                name: "FK_PatientProfile_User_UserId",
                table: "PatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PatientProfile_PatientProfileId",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "IsBook",
                table: "AvailableSlots",
                newName: "IsBooked");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientProfileId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AvailableSlotsId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sessions_AvailableSlots_AvailableSlotsId",
                        column: x => x.AvailableSlotsId,
                        principalTable: "AvailableSlots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_PatientProfile_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AvailableSlotsId",
                table: "Sessions",
                column: "AvailableSlotsId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DoctorId",
                table: "Sessions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PatientProfileId",
                table: "Sessions",
                column: "PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailableSlots_Doctor_DoctorId",
                table: "AvailableSlots",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certification_Doctor_DoctorId",
                table: "Certification",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Doctor_DoctorId",
                table: "Chat",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_PatientProfile_PatientProfileId",
                table: "Chat",
                column: "PatientProfileId",
                principalTable: "PatientProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Chat_chatId",
                table: "Message",
                column: "chatId",
                principalTable: "Chat",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientProfile_User_UserId",
                table: "PatientProfile",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PatientProfile_PatientProfileId",
                table: "Review",
                column: "PatientProfileId",
                principalTable: "PatientProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailableSlots_Doctor_DoctorId",
                table: "AvailableSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_Certification_Doctor_DoctorId",
                table: "Certification");

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
                name: "FK_PatientProfile_User_UserId",
                table: "PatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PatientProfile_PatientProfileId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.RenameColumn(
                name: "IsBooked",
                table: "AvailableSlots",
                newName: "IsBook");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailableSlots_Doctor_DoctorId",
                table: "AvailableSlots",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certification_Doctor_DoctorId",
                table: "Certification",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_PatientProfile_User_UserId",
                table: "PatientProfile",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Doctor_DoctorId",
                table: "Review",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PatientProfile_PatientProfileId",
                table: "Review",
                column: "PatientProfileId",
                principalTable: "PatientProfile",
                principalColumn: "Id");
        }
    }
}
