using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.Mutils.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectionEntrySpherePerks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpherePerks",
                columns: table => new
                {
                    CollectionEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Perk1 = table.Column<int>(type: "integer", nullable: false),
                    Perk2 = table.Column<int>(type: "integer", nullable: false),
                    Perk3 = table.Column<int>(type: "integer", nullable: false),
                    Perk4 = table.Column<int>(type: "integer", nullable: false),
                    Perk5 = table.Column<int>(type: "integer", nullable: false),
                    Perk6 = table.Column<bool>(type: "boolean", nullable: false),
                    Perk7 = table.Column<bool>(type: "boolean", nullable: false),
                    Perk8 = table.Column<bool>(type: "boolean", nullable: false),
                    Perk9 = table.Column<bool>(type: "boolean", nullable: false),
                    Perk10 = table.Column<bool>(type: "boolean", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpherePerks", x => x.CollectionEntryId);
                    table.ForeignKey(
                        name: "FK_SpherePerks_CollectionEntries_CollectionEntryId",
                        column: x => x.CollectionEntryId,
                        principalTable: "CollectionEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpherePerks_CollectionEntryId",
                table: "SpherePerks",
                column: "CollectionEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpherePerks");
        }
    }
}
