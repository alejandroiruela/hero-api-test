using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hero_api_test.Migrations
{
    /// <inheritdoc />
    public partial class AddingVillainTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilities_Heroes_HeroId",
                table: "Habilities"
            );

            migrationBuilder.AlterColumn<int>(
                name: "HeroId",
                table: "Habilities",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.AddColumn<int>(
                name: "VillainId",
                table: "Habilities",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Villains",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Villain_Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villains", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Habilities_VillainId",
                table: "Habilities",
                column: "VillainId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Habilities_Heroes_HeroId",
                table: "Habilities",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Habilities_Villains_VillainId",
                table: "Habilities",
                column: "VillainId",
                principalTable: "Villains",
                principalColumn: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habilities_Heroes_HeroId",
                table: "Habilities"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Habilities_Villains_VillainId",
                table: "Habilities"
            );

            migrationBuilder.DropTable(name: "Villains");

            migrationBuilder.DropIndex(name: "IX_Habilities_VillainId", table: "Habilities");

            migrationBuilder.DropColumn(name: "VillainId", table: "Habilities");

            migrationBuilder.AlterColumn<int>(
                name: "HeroId",
                table: "Habilities",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Habilities_Heroes_HeroId",
                table: "Habilities",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
