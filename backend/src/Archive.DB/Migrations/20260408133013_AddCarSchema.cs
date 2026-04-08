using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.TheArchive.DB.Migrations {
    /// <inheritdoc />
    public partial class AddCarSchema : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Brands", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                    BrandID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Models", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Models_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandID = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelID = table.Column<Guid>(type: "uuid", nullable: false),
                    Variant = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(4095)", maxLength: 4095, nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false),
                    Doors = table.Column<int>(type: "integer", nullable: false),
                    PriceEur = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MileageKm = table.Column<int>(type: "integer", nullable: true),
                    PowerKw = table.Column<int>(type: "integer", nullable: true),
                    Damaged = table.Column<bool>(type: "boolean", nullable: false),
                    Condition = table.Column<int>(type: "integer", nullable: false),
                    BodyType = table.Column<int>(type: "integer", nullable: true),
                    FuelType = table.Column<int>(type: "integer", nullable: true),
                    Transmission = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<int>(type: "integer", nullable: true),
                    InteriorColor = table.Column<int>(type: "integer", nullable: true),
                    InteriorType = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Cars", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cars_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cars_Models_ModelID",
                        column: x => x.ModelID,
                        principalTable: "Models",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarImage",
                columns: table => new {
                    CarsID = table.Column<Guid>(type: "uuid", nullable: false),
                    ImagesID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_CarImage", x => new { x.CarsID, x.ImagesID });
                    table.ForeignKey(
                        name: "FK_CarImage_Cars_CarsID",
                        column: x => x.CarsID,
                        principalTable: "Cars",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarImage_Images_ImagesID",
                        column: x => x.ImagesID,
                        principalTable: "Images",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarImage_ImagesID",
                table: "CarImage",
                column: "ImagesID");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_BrandID",
                table: "Cars",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_ModelID",
                table: "Cars",
                column: "ModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Models_BrandID",
                table: "Models",
                column: "BrandID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "CarImage");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Brands");
        }
    }
}
