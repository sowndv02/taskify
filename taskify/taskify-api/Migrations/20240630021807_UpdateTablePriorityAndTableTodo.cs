using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskify_api.Migrations
{
    public partial class UpdateTablePriorityAndTableTodo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Todos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Todos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Priorities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_PriorityId",
                table: "Todos",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Priorities_UserId",
                table: "Priorities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Priorities_Users_UserId",
                table: "Priorities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Priorities_PriorityId",
                table: "Todos",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Priorities_Users_UserId",
                table: "Priorities");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Priorities_PriorityId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_PriorityId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Priorities_UserId",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Priorities");
        }
    }
}
