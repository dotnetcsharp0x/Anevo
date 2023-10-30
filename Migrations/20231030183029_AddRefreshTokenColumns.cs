using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anevo.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "SU_001",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "SU_001",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "SU_001");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "SU_001");
        }
    }
}
