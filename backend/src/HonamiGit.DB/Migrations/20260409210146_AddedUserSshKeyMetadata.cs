using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shiron.HonamiGit.DB.Migrations {
    /// <inheritdoc />
    public partial class AddedUserSshKeyMetadata : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserSSHKeys",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "UserSSHKeys",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserSSHKeys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserSSHKeys");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "UserSSHKeys");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserSSHKeys");
        }
    }
}
