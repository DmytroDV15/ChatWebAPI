using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorAndUsersList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_RegisterModels_RegisterModelId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "RegisterModelId",
                table: "Chats",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_RegisterModelId",
                table: "Chats",
                newName: "IX_Chats_CreatorId");

            migrationBuilder.CreateTable(
                name: "ChatUser",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser", x => new { x.ChatId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ChatUser_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUser_RegisterModels_UserId",
                        column: x => x.UserId,
                        principalTable: "RegisterModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UserId",
                table: "ChatUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_RegisterModels_CreatorId",
                table: "Chats",
                column: "CreatorId",
                principalTable: "RegisterModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_RegisterModels_CreatorId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "ChatUser");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Chats",
                newName: "RegisterModelId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_CreatorId",
                table: "Chats",
                newName: "IX_Chats_RegisterModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_RegisterModels_RegisterModelId",
                table: "Chats",
                column: "RegisterModelId",
                principalTable: "RegisterModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
