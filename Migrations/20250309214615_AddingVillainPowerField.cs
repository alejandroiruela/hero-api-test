using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hero_api_test.Migrations
{
    /// <inheritdoc />
    public partial class AddingVillainPowerField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Power",
                table: "Villains",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Power", table: "Villains");
        }
    }
}
