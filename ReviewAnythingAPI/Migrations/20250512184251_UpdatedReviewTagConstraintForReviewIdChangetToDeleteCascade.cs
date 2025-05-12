using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewAnythingAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReviewTagConstraintForReviewIdChangetToDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTags_Reviews_ReviewId",
                table: "ReviewTags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTags_Reviews_ReviewId",
                table: "ReviewTags",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "ReviewId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTags_Reviews_ReviewId",
                table: "ReviewTags");

            migrationBuilder.AlterColumn<string>(
                name: "CreationDate",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTags_Reviews_ReviewId",
                table: "ReviewTags",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "ReviewId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
