using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.Mutils.Infrastructure.Migrations {
    /// <inheritdoc />
    public partial class AddIsFavoriteToCollectionEntry : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CollectionEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionEntries_IsFavorite",
                table: "CollectionEntries",
                column: "IsFavorite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_CollectionEntries_IsFavorite",
                table: "CollectionEntries");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CollectionEntries");
        }
    }
}
