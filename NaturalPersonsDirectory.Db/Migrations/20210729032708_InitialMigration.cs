using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NaturalPersonsDirectory.Db.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NaturalPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstNameGe = table.Column<string>(nullable: true),
                    FirstNameEn = table.Column<string>(nullable: true),
                    LastNameGe = table.Column<string>(nullable: true),
                    LastNameEn = table.Column<string>(nullable: true),
                    PassportNumber = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(type: "date", nullable: false),
                    Address = table.Column<string>(nullable: true),
                    ContactInformation = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromId = table.Column<int>(nullable: false),
                    ToId = table.Column<int>(nullable: false),
                    RelationType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relations_NaturalPersons_FromId",
                        column: x => x.FromId,
                        principalTable: "NaturalPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Relations_NaturalPersons_ToId",
                        column: x => x.ToId,
                        principalTable: "NaturalPersons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relations_FromId",
                table: "Relations",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Relations_ToId",
                table: "Relations",
                column: "ToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "NaturalPersons");
        }
    }
}
