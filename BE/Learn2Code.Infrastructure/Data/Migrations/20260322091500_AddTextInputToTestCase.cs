using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn2Code.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTextInputToTestCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "text_input",
                table: "test_cases",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "text_input",
                table: "test_cases");
        }
    }
}
