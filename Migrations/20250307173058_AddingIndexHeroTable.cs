using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hero_api_test.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexHeroTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Hero_name",
                table: "Heroes",
                column: "Hero_name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Heroes_Hero_name", table: "Heroes");
        }
    }
}
