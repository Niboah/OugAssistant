using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class ReworkModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeekTimes",
                table: "OugTasks",
                newName: "Routines");

            migrationBuilder.RenameColumn(
                name: "FinishDateTime",
                table: "OugTasks",
                newName: "FinishedDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "OugTasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTaskId",
                table: "OugTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoutineDays",
                table: "OugTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "OugGoal",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OugTasks_ParentTaskId",
                table: "OugTasks",
                column: "ParentTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugTasks_ParentTaskId",
                table: "OugTasks",
                column: "ParentTaskId",
                principalTable: "OugTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugTasks_ParentTaskId",
                table: "OugTasks");

            migrationBuilder.DropIndex(
                name: "IX_OugTasks_ParentTaskId",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "ParentTaskId",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "RoutineDays",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "OugGoal");

            migrationBuilder.RenameColumn(
                name: "Routines",
                table: "OugTasks",
                newName: "WeekTimes");

            migrationBuilder.RenameColumn(
                name: "FinishedDateTime",
                table: "OugTasks",
                newName: "FinishDateTime");
        }
    }
}
