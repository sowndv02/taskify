using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskify_api.Migrations
{
    public partial class UpdateTableWorkspaceColumnDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Workspaces",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Status",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Colors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WorkspaceId1",
                table: "ActivityLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Todos_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkspaceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_UserId",
                table: "Status",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_UserId",
                table: "Colors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_WorkspaceId1",
                table: "ActivityLogs",
                column: "WorkspaceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ColorId",
                table: "Notes",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_WorkspaceId",
                table: "Notes",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_UserId",
                table: "Todos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_WorkspaceId",
                table: "Todos",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_UserId",
                table: "WorkspaceUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceId",
                table: "WorkspaceUsers",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Workspaces_WorkspaceId1",
                table: "ActivityLogs",
                column: "WorkspaceId1",
                principalTable: "Workspaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_Users_UserId",
                table: "Colors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Status_Users_UserId",
                table: "Status",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Workspaces_WorkspaceId1",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Colors_Users_UserId",
                table: "Colors");

            migrationBuilder.DropForeignKey(
                name: "FK_Status_Users_UserId",
                table: "Status");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_UserId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "WorkspaceUsers");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Status_UserId",
                table: "Status");

            migrationBuilder.DropIndex(
                name: "IX_Colors_UserId",
                table: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_WorkspaceId1",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "WorkspaceId1",
                table: "ActivityLogs");
        }
    }
}
