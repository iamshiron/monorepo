using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.ResonanceSystem.DB.Migrations {
    /// <inheritdoc />
    public partial class AddedEchoAndEchoSonataTables : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Echoes",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Echoes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EchoSonatas",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EchoSonatas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EchoSonataLinks",
                columns: table => new {
                    EchoesID = table.Column<Guid>(type: "uuid", nullable: false),
                    SonatasID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EchoSonataLinks", x => new { x.EchoesID, x.SonatasID });
                    table.ForeignKey(
                        name: "FK_EchoSonataLinks_EchoSonatas_SonatasID",
                        column: x => x.SonatasID,
                        principalTable: "EchoSonatas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EchoSonataLinks_Echoes_EchoesID",
                        column: x => x.EchoesID,
                        principalTable: "Echoes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EchoSonataLinks_SonatasID",
                table: "EchoSonataLinks",
                column: "SonatasID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "EchoSonataLinks");

            migrationBuilder.DropTable(
                name: "EchoSonatas");

            migrationBuilder.DropTable(
                name: "Echoes");
        }
    }
}
