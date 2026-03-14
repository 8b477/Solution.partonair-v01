using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class AnnouncementEntityConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.ToTable("Announcements");

            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(a => a.JobTag)
                .HasColumnName("job_tag")
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(a => a.SkillsRequired)
                .HasColumnName("skills_required")
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(a => a.AnnouncementDescription)
                .HasColumnName("announcement_description")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(a => a.AnnouncementCreatedAt)
                .HasColumnName("announcement_created_at")
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(a => a.AnnouncementUpdatedAt)
                .HasColumnName("announcement_updated_at")
                .HasColumnType("DATETIME");

            builder.Property(a => a.UserId)
                .HasColumnName("fk_announcement_user")
                .IsRequired();

            // Relationship (User 1 → N Announcements)
            builder.HasOne(a => a.User)
                .WithMany(u => u.Announcements)
                .HasForeignKey(a => a.UserId)
                .HasConstraintName("FK_ANNOUNCEMENT_USER")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}