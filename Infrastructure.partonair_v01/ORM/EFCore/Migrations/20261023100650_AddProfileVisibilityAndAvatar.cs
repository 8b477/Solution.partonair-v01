using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileVisibilityAndAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "avatar_regen_count",
                table: "ProfileUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "avatar_regen_date",
                table: "ProfileUsers",
                type: "DATE",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "avatar_seed",
                table: "ProfileUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValueSql: "CONVERT(NVARCHAR(64), NEWID())");

            migrationBuilder.AddColumn<bool>(
                name: "show_self_in_directory",
                table: "ProfileUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_regen_count",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "avatar_regen_date",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "avatar_seed",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "show_self_in_directory",
                table: "ProfileUsers");
        }
    }
}
