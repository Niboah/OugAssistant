using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class RemakeModel4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeDay",
                table: "OugRoutine");

            migrationBuilder.DropColumn(
                name: "WeekDay",
                table: "OugRoutine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeDay",
                table: "OugRoutine",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "WeekDay",
                table: "OugRoutine",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
