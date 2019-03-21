using Microsoft.EntityFrameworkCore.Migrations;

namespace sample_app.Migrations
{
    public partial class AddHashConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "passHashIterations",
                table: "users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "passHashSize",
                table: "users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "passSaltSize",
                table: "users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passHashIterations",
                table: "users");

            migrationBuilder.DropColumn(
                name: "passHashSize",
                table: "users");

            migrationBuilder.DropColumn(
                name: "passSaltSize",
                table: "users");
        }
    }
}
