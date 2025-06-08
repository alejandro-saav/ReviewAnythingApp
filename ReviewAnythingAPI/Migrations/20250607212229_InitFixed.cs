using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewAnythingAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemId1",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId1",
                table: "Follows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ItemId1",
                table: "Reviews",
                column: "ItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_ApplicationUserId1",
                table: "Follows",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_ApplicationUserId1",
                table: "Follows",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Items_ItemId1",
                table: "Reviews",
                column: "ItemId1",
                principalTable: "Items",
                principalColumn: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_ApplicationUserId1",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Items_ItemId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ItemId1",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Follows_ApplicationUserId1",
                table: "Follows");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Follows");
        }
    }
}
