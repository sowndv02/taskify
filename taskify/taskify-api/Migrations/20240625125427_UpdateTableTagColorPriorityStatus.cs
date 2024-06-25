using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace taskify_api.Migrations
{
    public partial class UpdateTableTagColorPriorityStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Status",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Priorities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Colors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Colors");
        }
    }
}
