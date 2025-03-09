using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hero_api_test.Migrations
{
    /// <inheritdoc />
    public partial class AddingIndexVillainTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Villains_Villain_Name",
                table: "Villains",
                column: "Villain_Name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Villains_Villain_Name", table: "Villains");
        }
    }
}
