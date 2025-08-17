using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class NewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugGoal_OugGoal_ParentGoalId",
                table: "OugGoal");

            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugGoal_GoalId",
                table: "OugTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugTasks_ParentTaskId",
                table: "OugTasks");

            migrationBuilder.DropIndex(
                name: "IX_OugTasks_GoalId",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "OugTasks");

            migrationBuilder.RenameColumn(
                name: "ParentTaskId",
                table: "OugTasks",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_OugTasks_ParentTaskId",
                table: "OugTasks",
                newName: "IX_OugTasks_ParentId");

            migrationBuilder.RenameColumn(
                name: "ParentGoalId",
                table: "OugGoal",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_OugGoal_ParentGoalId",
                table: "OugGoal",
                newName: "IX_OugGoal_ParentId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "OugTasks",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OugTaskGoal",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OugTaskGoal", x => new { x.TaskId, x.GoalId });
                    table.ForeignKey(
                        name: "FK_OugTaskGoal_OugGoal_GoalId",
                        column: x => x.GoalId,
                        principalTable: "OugGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OugTaskGoal_OugTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "OugTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OugTaskGoal_GoalId",
                table: "OugTaskGoal",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OugGoal_OugGoal_ParentId",
                table: "OugGoal",
                column: "ParentId",
                principalTable: "OugGoal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugTasks_ParentId",
                table: "OugTasks",
                column: "ParentId",
                principalTable: "OugTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugGoal_OugGoal_ParentId",
                table: "OugGoal");

            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugTasks_ParentId",
                table: "OugTasks");

            migrationBuilder.DropTable(
                name: "OugTaskGoal");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "OugTasks");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "OugTasks",
                newName: "ParentTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_OugTasks_ParentId",
                table: "OugTasks",
                newName: "IX_OugTasks_ParentTaskId");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "OugGoal",
                newName: "ParentGoalId");

            migrationBuilder.RenameIndex(
                name: "IX_OugGoal_ParentId",
                table: "OugGoal",
                newName: "IX_OugGoal_ParentGoalId");

            migrationBuilder.AddColumn<Guid>(
                name: "GoalId",
                table: "OugTasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "OugTasks",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OugTasks_GoalId",
                table: "OugTasks",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OugGoal_OugGoal_ParentGoalId",
                table: "OugGoal",
                column: "ParentGoalId",
                principalTable: "OugGoal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugGoal_GoalId",
                table: "OugTasks",
                column: "GoalId",
                principalTable: "OugGoal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugTasks_ParentTaskId",
                table: "OugTasks",
                column: "ParentTaskId",
                principalTable: "OugTasks",
                principalColumn: "Id");
        }
    }
}
