using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace ReviewAnythingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchVectorToReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "Reviews",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "simple")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Content" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SearchVector",
                table: "Reviews",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_SearchVector",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Reviews");
        }
    }
}
