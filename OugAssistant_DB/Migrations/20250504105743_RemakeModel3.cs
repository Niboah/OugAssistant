using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class RemakeModel3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Routines_RoutineId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Routines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "OugTasks");

            migrationBuilder.RenameColumn(
                name: "DoDateTime",
                table: "OugTasks",
                newName: "RoutineDateTime");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "OugTasks",
                newName: "EventDateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_RoutineId",
                table: "OugTasks",
                newName: "IX_OugTasks_RoutineId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_GoalId",
                table: "OugTasks",
                newName: "IX_OugTasks_GoalId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OugTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OugTasks",
                table: "OugTasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OugGoal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OugGoal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OugRoutine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekDay = table.Column<int>(type: "int", nullable: false),
                    TimeDay = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OugRoutine", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugGoal_GoalId",
                table: "OugTasks",
                column: "GoalId",
                principalTable: "OugGoal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugRoutine_RoutineId",
                table: "OugTasks",
                column: "RoutineId",
                principalTable: "OugRoutine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugGoal_GoalId",
                table: "OugTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugRoutine_RoutineId",
                table: "OugTasks");

            migrationBuilder.DropTable(
                name: "OugGoal");

            migrationBuilder.DropTable(
                name: "OugRoutine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OugTasks",
                table: "OugTasks");

            migrationBuilder.RenameTable(
                name: "OugTasks",
                newName: "Tasks");

            migrationBuilder.RenameColumn(
                name: "RoutineDateTime",
                table: "Tasks",
                newName: "DoDateTime");

            migrationBuilder.RenameColumn(
                name: "EventDateTime",
                table: "Tasks",
                newName: "DateTime");

            migrationBuilder.RenameIndex(
                name: "IX_OugTasks_RoutineId",
                table: "Tasks",
                newName: "IX_Tasks_RoutineId");

            migrationBuilder.RenameIndex(
                name: "IX_OugTasks_GoalId",
                table: "Tasks",
                newName: "IX_Tasks_GoalId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeDay = table.Column<TimeOnly>(type: "time", nullable: false),
                    WeekDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routines", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Routines_RoutineId",
                table: "Tasks",
                column: "RoutineId",
                principalTable: "Routines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
