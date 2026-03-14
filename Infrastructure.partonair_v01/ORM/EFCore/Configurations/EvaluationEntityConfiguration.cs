using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class EvaluationEntityConfiguration : IEntityTypeConfiguration<Evaluation>
    {
        public void Configure(EntityTypeBuilder<Evaluation> builder)
        {
            builder.ToTable("Evaluations");

            // Primary Key
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(e => e.EvaluationCommentary)
                .HasColumnName("evaluation_commentary")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.EvaluationCreatedAt)
                .HasColumnName("evaluation_created_at")
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.EvaluationUpdatedAt)
                .HasColumnName("evaluation_updated_at")
                .HasColumnType("DATETIME");

            builder.Property(e => e.EvaluationValue)
                .HasColumnName("evaluation_value")
                .HasColumnType("TINYINT")
                .IsRequired();

            builder.Property(e => e.EvalReceiverId)
                .HasColumnName("fk_eval_receiver")
                .IsRequired();

            builder.Property(e => e.EvalSenderId)
                .HasColumnName("fk_eval_sender")
                .IsRequired();

            // Relation Receiver
            builder.HasOne(e => e.Receiver)
                .WithMany(u => u.EvaluationsReceived)
                .HasForeignKey(e => e.EvalReceiverId)
                .HasConstraintName("FK_EVAL_RECEIVER")
                .OnDelete(DeleteBehavior.Restrict);

            // Relation Sender
            builder.HasOne(e => e.Sender)
                .WithMany(u => u.EvaluationsSent)
                .HasForeignKey(e => e.EvalSenderId)
                .HasConstraintName("FK_EVAL_SENDER")
                .OnDelete(DeleteBehavior.Restrict);

            // Check constraint
            builder.ToTable(t => t.HasCheckConstraint("CK_Announcements_EvaluationValue_Valid", "evaluation_value BETWEEN 0 AND 5"));
        }
    }
}