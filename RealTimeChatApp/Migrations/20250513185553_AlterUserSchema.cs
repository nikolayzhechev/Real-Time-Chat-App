using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatApp.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AppUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "AppUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "AppUsers");
        }
    }
}
