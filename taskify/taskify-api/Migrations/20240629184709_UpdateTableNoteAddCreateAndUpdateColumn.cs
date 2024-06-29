using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskify_api.Migrations
{
    public partial class UpdateTableNoteAddCreateAndUpdateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Notes");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTags_Projects_ProjectId",
                table: "ProjectTags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
