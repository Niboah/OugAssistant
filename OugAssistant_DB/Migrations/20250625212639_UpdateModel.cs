using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OugAssistant_DB.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "OugGoal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentGoalId",
                table: "OugGoal",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OugGoal_ParentGoalId",
                table: "OugGoal",
                column: "ParentGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OugGoal_OugGoal_ParentGoalId",
                table: "OugGoal",
                column: "ParentGoalId",
                principalTable: "OugGoal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OugGoal_OugGoal_ParentGoalId",
                table: "OugGoal");

            migrationBuilder.DropIndex(
                name: "IX_OugGoal_ParentGoalId",
                table: "OugGoal");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "OugGoal");

            migrationBuilder.DropColumn(
                name: "ParentGoalId",
                table: "OugGoal");
        }
    }
}
