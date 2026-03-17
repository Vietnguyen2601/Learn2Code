using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learn2Code.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInstructorIdFromCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_accounts_instructor_id",
                table: "courses");

            migrationBuilder.DropIndex(
                name: "IX_courses_instructor_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "instructor_id",
                table: "courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "instructor_id",
                table: "courses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_courses_instructor_id",
                table: "courses",
                column: "instructor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_accounts_instructor_id",
                table: "courses",
                column: "instructor_id",
                principalTable: "accounts",
                principalColumn: "account_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
