using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    mail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    password_hashed = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Visitor"),
                    user_created_at = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    user_updated_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    last_connection = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.CheckConstraint("CK_Users_Role_Valid", "role IN ('Visitor', 'Admin', 'Moderator')");
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    job_tag = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    skills_required = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    announcement_description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    announcement_created_at = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    announcement_updated_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    fk_announcement_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.id);
                    table.ForeignKey(
                        name: "FK_ANNOUNCEMENT_USER",
                        column: x => x.fk_announcement_user,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    contact_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    contact_mail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    is_friendly = table.Column<bool>(type: "bit", nullable: true),
                    accepted_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    blocked_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    is_blocked = table.Column<bool>(type: "bit", nullable: true),
                    contact_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    fk_contact_receiver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fk_contact_sender = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.id);
                    table.CheckConstraint("CK_Contacts_NoSelf", "[fk_contact_sender] <> [fk_contact_receiver]");
                    table.CheckConstraint("CK_Contacts_Status_Valid", "[contact_status] IN ('Pending','Accepted','Blocked','Refused')");
                    table.ForeignKey(
                        name: "FK_CONTACT_RECEIVER",
                        column: x => x.fk_contact_receiver,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONTACT_SENDER",
                        column: x => x.fk_contact_sender,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    evaluation_commentary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    evaluation_created_at = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    evaluation_updated_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    evaluation_value = table.Column<byte>(type: "TINYINT", nullable: false),
                    fk_eval_receiver = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fk_eval_sender = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.id);
                    table.CheckConstraint("CK_Announcements_EvaluationValue_Valid", "evaluation_value BETWEEN 0 AND 5");
                    table.ForeignKey(
                        name: "FK_EVAL_RECEIVER",
                        column: x => x.fk_eval_receiver,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVAL_SENDER",
                        column: x => x.fk_eval_sender,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    profile_description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    skills = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    url_cv = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_public = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    stars = table.Column<byte>(type: "TINYINT", nullable: false),
                    profile_created_at = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETDATE()"),
                    profile_updated_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    fk_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_USER",
                        column: x => x.fk_user,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    profile_image_bits = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    type_profile_image = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    size_profile_image = table.Column<int>(type: "int", nullable: true),
                    cv_image_bits = table.Column<byte[]>(type: "VARBINARY(MAX)", nullable: true),
                    type_cv_image = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    size_cv_image = table.Column<int>(type: "int", nullable: true),
                    fk_profile_user = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.id);
                    table.CheckConstraint("CK_type_cv_image", "[type_cv_image] IN ('.jpg','.png','.svg','.jpeg','.webp','.pdf')");
                    table.CheckConstraint("CK_type_profile_image", "[type_profile_image] IN ('.jpg','.png','.svg','.jpeg','.webp')");
                    table.ForeignKey(
                        name: "FK_PROFILE_USER",
                        column: x => x.fk_profile_user,
                        principalTable: "ProfileUsers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_fk_announcement_user",
                table: "Announcements",
                column: "fk_announcement_user");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_fk_contact_receiver",
                table: "Contacts",
                column: "fk_contact_receiver");

            migrationBuilder.CreateIndex(
                name: "UQ_Contacts_Sender_Receiver",
                table: "Contacts",
                columns: new[] { "fk_contact_sender", "fk_contact_receiver" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_fk_eval_receiver",
                table: "Evaluations",
                column: "fk_eval_receiver");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_fk_eval_sender",
                table: "Evaluations",
                column: "fk_eval_sender");

            migrationBuilder.CreateIndex(
                name: "IX_Images_fk_profile_user",
                table: "Images",
                column: "fk_profile_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUsers_fk_user",
                table: "ProfileUsers",
                column: "fk_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_mail",
                table: "Users",
                column: "mail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "ProfileUsers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
