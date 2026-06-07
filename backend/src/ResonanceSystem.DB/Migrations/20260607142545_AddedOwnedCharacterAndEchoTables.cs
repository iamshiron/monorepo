using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.ResonanceSystem.DB.Migrations {
    /// <inheritdoc />
    public partial class AddedOwnedCharacterAndEchoTables : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "OwnedCharacters",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterID = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    UserID = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceChain = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Forte0Level = table.Column<int>(type: "integer", nullable: false),
                    Forte1Level = table.Column<int>(type: "integer", nullable: false),
                    Forte2Level = table.Column<int>(type: "integer", nullable: false),
                    Forte3Level = table.Column<int>(type: "integer", nullable: false),
                    Forte4Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OwnedCharacters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OwnedCharacters_Characters_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedCharacters_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnedEchos",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false),
                    MainStatType = table.Column<int>(type: "integer", nullable: false),
                    MainStatValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    CharacterID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OwnedEchos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OwnedEchos_OwnedCharacters_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "OwnedCharacters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EchoSubStats",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    EchoID = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_EchoSubStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EchoSubStats_OwnedEchos_EchoID",
                        column: x => x.EchoID,
                        principalTable: "OwnedEchos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EchoSubStats_EchoID",
                table: "EchoSubStats",
                column: "EchoID");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedCharacters_CharacterID",
                table: "OwnedCharacters",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedCharacters_UserID",
                table: "OwnedCharacters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedEchos_CharacterID",
                table: "OwnedEchos",
                column: "CharacterID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "EchoSubStats");

            migrationBuilder.DropTable(
                name: "OwnedEchos");

            migrationBuilder.DropTable(
                name: "OwnedCharacters");
        }
    }
}
