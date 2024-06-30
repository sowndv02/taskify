using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskify_api.Migrations
{
    public partial class UpdateTableTaskDelColumnPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Priorities_PriorityId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_PriorityId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Task_PriorityId",
                table: "Task",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Priorities_PriorityId",
                table: "Task",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
