using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.TheArchive.DB.Migrations {
    /// <inheritdoc />
    public partial class AddSchemaEntities : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Alias = table.Column<List<string>>(type: "text[]", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedByID = table.Column<Guid>(type: "uuid", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Characters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Characters_Users_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Studios",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedByID = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Studios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Studios_Users_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Bucket = table.Column<string>(type: "text", nullable: false),
                    ObjectKey = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    BlurHash = table.Column<string>(type: "text", nullable: false),
                    PrimaryColor_Color = table.Column<int>(type: "integer", nullable: false),
                    PrimaryColor_Lab_L = table.Column<double>(type: "double precision", nullable: false),
                    PrimaryColor_Lab_A = table.Column<double>(type: "double precision", nullable: false),
                    PrimaryColor_Lab_B = table.Column<double>(type: "double precision", nullable: false),
                    SecondaryColor_Color = table.Column<int>(type: "integer", nullable: false),
                    SecondaryColor_Lab_L = table.Column<double>(type: "double precision", nullable: false),
                    SecondaryColor_Lab_A = table.Column<double>(type: "double precision", nullable: false),
                    SecondaryColor_Lab_B = table.Column<double>(type: "double precision", nullable: false),
                    CharacterID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Palette = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Images", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Images_Characters_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Synopsis = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: false),
                    EpisodeCount = table.Column<int>(type: "integer", nullable: false),
                    WideBannerID = table.Column<Guid>(type: "uuid", nullable: true),
                    SquareBannerID = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StudioID = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByID = table.Column<Guid>(type: "uuid", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Medias", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Medias_Images_SquareBannerID",
                        column: x => x.SquareBannerID,
                        principalTable: "Images",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Medias_Images_WideBannerID",
                        column: x => x.WideBannerID,
                        principalTable: "Images",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Medias_Studios_StudioID",
                        column: x => x.StudioID,
                        principalTable: "Studios",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Medias_Users_CreatedByID",
                        column: x => x.CreatedByID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CharacterMedia",
                columns: table => new {
                    CharactersID = table.Column<Guid>(type: "uuid", nullable: false),
                    MediasID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CharacterMedia", x => new { x.CharactersID, x.MediasID });
                    table.ForeignKey(
                        name: "FK_CharacterMedia_Characters_CharactersID",
                        column: x => x.CharactersID,
                        principalTable: "Characters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMedia_Medias_MediasID",
                        column: x => x.MediasID,
                        principalTable: "Medias",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMedia_MediasID",
                table: "CharacterMedia",
                column: "MediasID");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedByID",
                table: "Characters",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CharacterID",
                table: "Images",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_CreatedByID",
                table: "Medias",
                column: "CreatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_SquareBannerID",
                table: "Medias",
                column: "SquareBannerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medias_StudioID",
                table: "Medias",
                column: "StudioID");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_WideBannerID",
                table: "Medias",
                column: "WideBannerID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Studios_CreatedByID",
                table: "Studios",
                column: "CreatedByID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "CharacterMedia");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Studios");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
