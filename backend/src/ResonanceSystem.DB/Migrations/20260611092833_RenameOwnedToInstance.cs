using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.ResonanceSystem.DB.Migrations;

public partial class RenameOwnedToInstance : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable("EchoSubStats");
        migrationBuilder.DropTable("OwnedEchos");
        migrationBuilder.DropTable("OwnedCharacters");

        migrationBuilder.CreateTable(
            "CharacterInstances",
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
                table.PrimaryKey("PK_CharacterInstances", x => x.ID);
                table.ForeignKey(
                    "FK_CharacterInstances_Characters_CharacterID",
                    x => x.CharacterID,
                    "Characters", "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_CharacterInstances_Users_UserID",
                    x => x.UserID,
                    "Users", "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "EchoInstances",
            columns: table => new {
                ID = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Level = table.Column<int>(type: "integer", nullable: false),
                Cost = table.Column<int>(type: "integer", nullable: false),
                MainStatType = table.Column<int>(type: "integer", nullable: false),
                MainStatValue = table.Column<decimal>(type: "numeric", nullable: false),
                Index = table.Column<int>(type: "integer", nullable: false),
                CharacterInstanceID = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_EchoInstances", x => x.ID);
                table.ForeignKey(
                    "FK_EchoInstances_CharacterInstances_CharacterInstanceID",
                    x => x.CharacterInstanceID,
                    "CharacterInstances", "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "EchoSubStats",
            columns: table => new {
                ID = table.Column<Guid>(type: "uuid", nullable: false),
                EchoInstanceID = table.Column<Guid>(type: "uuid", nullable: false),
                Index = table.Column<int>(type: "integer", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Value = table.Column<decimal>(type: "numeric", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_EchoSubStats", x => x.ID);
                table.ForeignKey(
                    "FK_EchoSubStats_EchoInstances_EchoInstanceID",
                    x => x.EchoInstanceID,
                    "EchoInstances", "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_CharacterInstances_CharacterID", "CharacterInstances", "CharacterID");
        migrationBuilder.CreateIndex("IX_CharacterInstances_UserID", "CharacterInstances", "UserID");
        migrationBuilder.CreateIndex("IX_EchoInstances_CharacterInstanceID", "EchoInstances", "CharacterInstanceID");
        migrationBuilder.CreateIndex("IX_EchoSubStats_EchoInstanceID", "EchoSubStats", "EchoInstanceID");
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable("EchoSubStats");
        migrationBuilder.DropTable("EchoInstances");
        migrationBuilder.DropTable("CharacterInstances");

        migrationBuilder.CreateTable(
            "OwnedCharacters",
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
                    "FK_OwnedCharacters_Characters_CharacterID",
                    x => x.CharacterID,
                    "Characters", "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "FK_OwnedCharacters_Users_UserID",
                    x => x.UserID,
                    "Users", "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "OwnedEchos",
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
                    "FK_OwnedEchos_OwnedCharacters_CharacterID",
                    x => x.CharacterID,
                    "OwnedCharacters", "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "EchoSubStats",
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
                    "FK_EchoSubStats_OwnedEchos_EchoID",
                    x => x.EchoID,
                    "OwnedEchos", "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_OwnedCharacters_CharacterID", "OwnedCharacters", "CharacterID");
        migrationBuilder.CreateIndex("IX_OwnedCharacters_UserID", "OwnedCharacters", "UserID");
        migrationBuilder.CreateIndex("IX_OwnedEchos_CharacterID", "OwnedEchos", "CharacterID");
        migrationBuilder.CreateIndex("IX_EchoSubStats_EchoID", "EchoSubStats", "EchoID");
    }
}
