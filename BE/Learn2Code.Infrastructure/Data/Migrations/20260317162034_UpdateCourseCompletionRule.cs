using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn2Code.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourseCompletionRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "min_exercise_pass_pct",
                table: "course_completion_rules");

            migrationBuilder.DropColumn(
                name: "min_lesson_completion_pct",
                table: "course_completion_rules");

            migrationBuilder.RenameColumn(
                name: "min_section_quiz_score",
                table: "course_completion_rules",
                newName: "min_weight_score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "min_weight_score",
                table: "course_completion_rules",
                newName: "min_section_quiz_score");

            migrationBuilder.AddColumn<decimal>(
                name: "min_exercise_pass_pct",
                table: "course_completion_rules",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "min_lesson_completion_pct",
                table: "course_completion_rules",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
