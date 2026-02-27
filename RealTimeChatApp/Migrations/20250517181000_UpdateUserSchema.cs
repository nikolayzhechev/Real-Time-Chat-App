using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RealTimeChatApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_AppUsers_UserId",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AppUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_UserId",
                table: "ChatUsers");

            migrationBuilder.AddColumn<int>(
                name: "SenderId1",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "ChatUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AppUsers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages",
                column: "SenderId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserId1",
                table: "ChatUsers",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_AppUsers_UserId1",
                table: "ChatUsers",
                column: "UserId1",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AppUsers_SenderId1",
                table: "Messages",
                column: "SenderId1",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_AppUsers_UserId1",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AppUsers_SenderId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_UserId1",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "SenderId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ChatUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AppUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserId",
                table: "ChatUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_AppUsers_UserId",
                table: "ChatUsers",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AppUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
