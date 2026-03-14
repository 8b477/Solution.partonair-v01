using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class ProfileUserEntityConfiguration : IEntityTypeConfiguration<ProfileUser>
    {
        public void Configure(EntityTypeBuilder<ProfileUser> builder)
        {
            builder.ToTable("ProfileUsers");

            // Primary Key
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(p => p.ProfileDescription)
                .HasColumnName("profile_description")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.Skills)
                .HasColumnName("skills")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.UrlCv)
                .HasColumnName("url_cv")
                .HasMaxLength(500);

            builder.Property(p => p.IsPublic)
                .HasColumnName("is_public")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(p => p.Stars)
                .HasColumnName("stars")
                .HasColumnType("TINYINT");

            builder.Property(p => p.ProfileCreatedAt)
                .HasColumnName("profile_created_at")
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(p => p.ProfileUpdatedAt)
                .HasColumnName("profile_updated_at")
                .HasColumnType("DATETIME");

            builder.Property(p => p.UserId)
                .HasColumnName("fk_user")
                .IsRequired();

            // Unique constraint
            builder.HasIndex(p => p.UserId)
                .IsUnique();

            // Relationship 1-1
            builder.HasOne(p => p.User)
                .WithOne(u => u.ProfileUser)
                .HasForeignKey<ProfileUser>(p => p.UserId)
                .HasConstraintName("FK_USER")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}