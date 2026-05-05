using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Role_Valid",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Role_Valid",
                table: "Users",
                sql: "role IN ('Visitor', 'Employee', 'Company', 'Admin', 'Moderator')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Role_Valid",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Role_Valid",
                table: "Users",
                sql: "role IN ('Visitor', 'Admin', 'Moderator')");
        }
    }
}
