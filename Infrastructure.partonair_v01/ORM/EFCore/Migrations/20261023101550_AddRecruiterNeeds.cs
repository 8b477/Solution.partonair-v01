using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruiterNeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "contract_types",
                table: "ProfileUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "ProfileUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "remote_policy",
                table: "ProfileUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sector",
                table: "ProfileUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sought_position",
                table: "ProfileUsers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contract_types",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "location",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "remote_policy",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "sector",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "sought_position",
                table: "ProfileUsers");
        }
    }
}
