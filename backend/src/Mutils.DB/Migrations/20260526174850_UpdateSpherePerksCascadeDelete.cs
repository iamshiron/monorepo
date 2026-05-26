using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.Mutils.DB.Migrations {
    /// <inheritdoc />
    public partial class UpdateSpherePerksCascadeDelete : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_SpherePerks_CollectionEntries_CollectionEntryId",
                table: "SpherePerks");

            migrationBuilder.AddForeignKey(
                name: "FK_SpherePerks_CollectionEntries_CollectionEntryId",
                table: "SpherePerks",
                column: "CollectionEntryId",
                principalTable: "CollectionEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_SpherePerks_CollectionEntries_CollectionEntryId",
                table: "SpherePerks");

            migrationBuilder.AddForeignKey(
                name: "FK_SpherePerks_CollectionEntries_CollectionEntryId",
                table: "SpherePerks",
                column: "CollectionEntryId",
                principalTable: "CollectionEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
