using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApp.Migrations
{
    /// <inheritdoc />
    public partial class Chatsentitynamingupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Chats",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DirectKey",
                table: "Chats",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Chats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_DirectKey",
                table: "Chats",
                column: "DirectKey",
                unique: true,
                filter: "\"IsGroup\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chats_DirectKey",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "DirectKey",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Chats",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
