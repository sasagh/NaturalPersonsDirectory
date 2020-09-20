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
                    NaturalPersonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstNameGE = table.Column<string>(nullable: true),
                    FirstNameEn = table.Column<string>(nullable: true),
                    LastNameGe = table.Column<string>(nullable: true),
                    LastNameEn = table.Column<string>(nullable: true),
                    PassportNumber = table.Column<string>(nullable: true),
                    Birthday = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    ContactInformations = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturalPersons", x => x.NaturalPersonId);
                });

            migrationBuilder.CreateTable(
                name: "RelationIds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationIds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    FromId = table.Column<int>(nullable: false),
                    ToId = table.Column<int>(nullable: false),
                    RelationId = table.Column<int>(nullable: false),
                    RelationType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => new { x.FromId, x.ToId });
                    table.ForeignKey(
                        name: "FK_Relations_NaturalPersons_FromId",
                        column: x => x.FromId,
                        principalTable: "NaturalPersons",
                        principalColumn: "NaturalPersonId");
                    table.ForeignKey(
                        name: "FK_Relations_RelationIds_RelationId",
                        column: x => x.RelationId,
                        principalTable: "RelationIds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Relations_NaturalPersons_ToId",
                        column: x => x.ToId,
                        principalTable: "NaturalPersons",
                        principalColumn: "NaturalPersonId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relations_RelationId",
                table: "Relations",
                column: "RelationId",
                unique: true);

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

            migrationBuilder.DropTable(
                name: "RelationIds");
        }
    }
}
