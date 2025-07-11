using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentChatIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentChatId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentChatId",
                table: "Users",
                column: "CurrentChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Chats_CurrentChatId",
                table: "Users",
                column: "CurrentChatId",
                principalTable: "Chats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Chats_CurrentChatId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurrentChatId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentChatId",
                table: "Users");
        }
    }
}
