using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anevo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTablesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SU001");

            migrationBuilder.DropTable(
                name: "SU010");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "SG_001",
                columns: table => new
                {
                    SG001_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SU001_Id_User = table.Column<int>(type: "int", nullable: false),
                    SG001_GroupNr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SG_001", x => x.SG001_Id);
                });

            migrationBuilder.CreateTable(
                name: "SG_010",
                columns: table => new
                {
                    SG010_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SG010_Group_Nr = table.Column<int>(type: "int", nullable: false),
                    SU010_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SG_010", x => x.SG010_Id);
                });

            migrationBuilder.CreateTable(
                name: "SU_001",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SU_001", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SG_001");

            migrationBuilder.DropTable(
                name: "SG_010");

            migrationBuilder.DropTable(
                name: "SU_001");

            migrationBuilder.CreateTable(
                name: "SU001",
                columns: table => new
                {
                    SU001_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SU001_GroupNr = table.Column<int>(type: "int", nullable: false),
                    SU001_Id_User = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }
    }
}
