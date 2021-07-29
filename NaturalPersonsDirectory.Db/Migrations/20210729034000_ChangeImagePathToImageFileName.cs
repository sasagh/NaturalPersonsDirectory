using Microsoft.EntityFrameworkCore.Migrations;

namespace NaturalPersonsDirectory.Db.Migrations
{
    public partial class ChangeImagePathToImageFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "NaturalPersons");

            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "NaturalPersons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "NaturalPersons");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "NaturalPersons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
