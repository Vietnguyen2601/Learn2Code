using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn2Code.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSubmitFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "validator_main",
                table: "test_cases",
                newName: "main_code");

            migrationBuilder.RenameColumn(
                name: "solution_validator",
                table: "exercises",
                newName: "default_main_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "main_code",
                table: "test_cases",
                newName: "validator_main");

            migrationBuilder.RenameColumn(
                name: "default_main_code",
                table: "exercises",
                newName: "solution_validator");
        }
    }
}
