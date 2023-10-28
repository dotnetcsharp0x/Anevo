using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anevo.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SU001",
                columns: table => new
                {
                    SU001_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SU001_Id_User = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SU001_GroupNr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SU001", x => x.SU001_Id);
                });

            migrationBuilder.CreateTable(
                name: "SU010",
                columns: table => new
                {
                    SU010_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SU010_Group_Nr = table.Column<int>(type: "int", nullable: false),
                    SU010_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SU010", x => x.SU010_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SU001");

            migrationBuilder.DropTable(
                name: "SU010");
        }
    }
}
