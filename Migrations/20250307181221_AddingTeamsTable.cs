using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hero_api_test.Migrations
{
    /// <inheritdoc />
    public partial class AddingTeamsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Villains",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Heroes",
                type: "integer",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    TeamName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Villains_TeamId",
                table: "Villains",
                column: "TeamId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_TeamId",
                table: "Heroes",
                column: "TeamId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamName",
                table: "Teams",
                column: "TeamName",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Heroes_Teams_TeamId",
                table: "Heroes",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Villains_Teams_TeamId",
                table: "Villains",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Heroes_Teams_TeamId", table: "Heroes");

            migrationBuilder.DropForeignKey(name: "FK_Villains_Teams_TeamId", table: "Villains");

            migrationBuilder.DropTable(name: "Teams");

            migrationBuilder.DropIndex(name: "IX_Villains_TeamId", table: "Villains");

            migrationBuilder.DropIndex(name: "IX_Heroes_TeamId", table: "Heroes");

            migrationBuilder.DropColumn(name: "TeamId", table: "Villains");

            migrationBuilder.DropColumn(name: "TeamId", table: "Heroes");
        }
    }
}
