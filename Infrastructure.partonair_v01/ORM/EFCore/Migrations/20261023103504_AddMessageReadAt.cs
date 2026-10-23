using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageReadAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "read_at",
                table: "Messages",
                type: "DATETIME2",
                nullable: true);

            // L'historique existant est considéré comme déjà lu : pas de badge rétroactif.
            migrationBuilder.Sql("UPDATE [Messages] SET [read_at] = [sent_at] WHERE [read_at] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Unread",
                table: "Messages",
                columns: new[] { "receiver_id", "read_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_Unread",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "read_at",
                table: "Messages");
        }
    }
}
