using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.partonair_v01.ORM.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evaluations_fk_eval_sender",
                table: "Evaluations");

            migrationBuilder.AddColumn<decimal>(
                name: "rating_average",
                table: "ProfileUsers",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "rating_count",
                table: "ProfileUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Données existantes : on ne garde que l'avis le plus récent par (auteur, évalué) avant l'index unique...
            migrationBuilder.Sql(@"
WITH ranked AS (
    SELECT id, ROW_NUMBER() OVER (PARTITION BY fk_eval_sender, fk_eval_receiver ORDER BY evaluation_created_at DESC) AS rn
    FROM [Evaluations]
)
DELETE FROM ranked WHERE rn > 1;");

            // ...puis on initialise la moyenne / le nombre d'avis des profils.
            migrationBuilder.Sql(@"
UPDATE p
SET p.rating_count = s.cnt,
    p.rating_average = s.average,
    p.stars = CAST(ROUND(s.average, 0) AS TINYINT)
FROM [ProfileUsers] p
JOIN (
    SELECT fk_eval_receiver, COUNT(*) AS cnt, CAST(AVG(CAST(evaluation_value AS DECIMAL(5,2))) AS DECIMAL(3,2)) AS average
    FROM [Evaluations]
    GROUP BY fk_eval_receiver
) s ON s.fk_eval_receiver = p.fk_user;");

            migrationBuilder.CreateIndex(
                name: "UX_Evaluations_Sender_Receiver",
                table: "Evaluations",
                columns: new[] { "fk_eval_sender", "fk_eval_receiver" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Evaluations_Sender_Receiver",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "rating_average",
                table: "ProfileUsers");

            migrationBuilder.DropColumn(
                name: "rating_count",
                table: "ProfileUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_fk_eval_sender",
                table: "Evaluations",
                column: "fk_eval_sender");
        }
    }
}
