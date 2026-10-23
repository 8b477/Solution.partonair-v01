using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    public partial class AddMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    sender_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    receiver_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    sent_at = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                    table.CheckConstraint("CK_Messages_NoSelf", "[sender_id] <> [receiver_id]");
                    table.CheckConstraint("CK_Messages_Content_NotEmpty", "LEN(LTRIM(RTRIM([content]))) > 0");
                    table.ForeignKey("FK_MESSAGES_SENDER", x => x.sender_id, "Users", "id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_MESSAGES_RECEIVER", x => x.receiver_id, "Users", "id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_Messages_Conversation", "Messages", new[] { "sender_id", "receiver_id", "sent_at" });
            migrationBuilder.CreateIndex("IX_Messages_Conversation_Reverse", "Messages", new[] { "receiver_id", "sender_id", "sent_at" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Messages");
        }
    }
}
