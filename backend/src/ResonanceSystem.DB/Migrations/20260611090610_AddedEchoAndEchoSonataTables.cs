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
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Echoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EchoSonatas",
                columns: table => new {
                    Id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EchoSonatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EchoSonataLinks",
                columns: table => new {
                    EchoesId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    SonatasId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EchoSonataLinks", x => new { x.EchoesId, x.SonatasId });
                    table.ForeignKey(
                        name: "FK_EchoSonataLinks_EchoSonatas_SonatasId",
                        column: x => x.SonatasId,
                        principalTable: "EchoSonatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EchoSonataLinks_Echoes_EchoesId",
                        column: x => x.EchoesId,
                        principalTable: "Echoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EchoSonataLinks_SonatasId",
                table: "EchoSonataLinks",
                column: "SonatasId");
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
