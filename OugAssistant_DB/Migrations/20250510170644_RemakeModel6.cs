using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class RemakeModel6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugTasks_OugRoutine_RoutineId",
                table: "OugTasks");

            migrationBuilder.DropTable(
                name: "OugRoutine");

            migrationBuilder.DropIndex(
                name: "IX_OugTasks_RoutineId",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "RoutineDateTime",
                table: "OugTasks");

            migrationBuilder.DropColumn(
                name: "RoutineId",
                table: "OugTasks");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "OugTasks",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<string>(
                name: "WeekTimes",
                table: "OugTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekTimes",
                table: "OugTasks");

            migrationBuilder.AlterColumn<string>(
                name: "TaskType",
                table: "OugTasks",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AddColumn<DateTime>(
                name: "RoutineDateTime",
                table: "OugTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoutineId",
                table: "OugTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OugRoutine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeDay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeekDay = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OugRoutine", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OugTasks_RoutineId",
                table: "OugTasks",
                column: "RoutineId");

            migrationBuilder.AddForeignKey(
                name: "FK_OugTasks_OugRoutine_RoutineId",
                table: "OugTasks",
                column: "RoutineId",
                principalTable: "OugRoutine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
