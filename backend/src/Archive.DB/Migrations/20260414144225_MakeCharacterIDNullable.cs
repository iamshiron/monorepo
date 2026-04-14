using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.TheArchive.DB.Migrations {
    /// <inheritdoc />
    public partial class MakeCharacterIDNullable : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Characters_CharacterID",
                table: "Images");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterID",
                table: "Images",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Characters_CharacterID",
                table: "Images",
                column: "CharacterID",
                principalTable: "Characters",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Characters_CharacterID",
                table: "Images");

            migrationBuilder.AlterColumn<Guid>(
                name: "CharacterID",
                table: "Images",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Characters_CharacterID",
                table: "Images",
                column: "CharacterID",
                principalTable: "Characters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
