using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class herencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutineTask_Routine_RoutineId",
                table: "RoutineTask");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Mission");

            migrationBuilder.DropTable(
                name: "Routine");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Task",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeadLine",
                table: "Task",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Task",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskType",
                table: "Task",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeDay",
                table: "Task",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeekDay",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoutineTask_Task_RoutineId",
                table: "RoutineTask",
                column: "RoutineId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoutineTask_Task_RoutineId",
                table: "RoutineTask");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "DeadLine",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "TaskType",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "TimeDay",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "WeekDay",
                table: "Task");

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Task_Id",
                        column: x => x.Id,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeadLine = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mission_Task_Id",
                        column: x => x.Id,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeDay = table.Column<TimeOnly>(type: "time", nullable: false),
                    WeekDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routine_Task_Id",
                        column: x => x.Id,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RoutineTask_Routine_RoutineId",
                table: "RoutineTask",
                column: "RoutineId",
                principalTable: "Routine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
