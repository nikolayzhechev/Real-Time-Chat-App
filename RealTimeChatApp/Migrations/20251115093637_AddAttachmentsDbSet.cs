using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Messages_MessageId",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_MessageId",
                table: "Attachments",
                newName: "IX_Attachments_MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ChatId",
                table: "Attachments",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Chats_ChatId",
                table: "Attachments",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Messages_MessageId",
                table: "Attachments",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Chats_ChatId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Messages_MessageId",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ChatId",
                table: "Attachments");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_MessageId",
                table: "Attachment",
                newName: "IX_Attachment_MessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Messages_MessageId",
                table: "Attachment",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
