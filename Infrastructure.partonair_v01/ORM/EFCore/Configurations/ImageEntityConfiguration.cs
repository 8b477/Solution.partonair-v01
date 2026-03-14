using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class ImageEntityConfiguration : IEntityTypeConfiguration<Images>
    {
        public void Configure(EntityTypeBuilder<Images> builder)
        {
            builder.ToTable("Images");

            // Primary key
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(i => i.ProfileImageBits)
                .HasColumnName("profile_image_bits")
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(i => i.TypeProfileImage)
                .HasColumnName("type_profile_image")
                .HasMaxLength(10);

            builder.Property(i => i.SizeProfileImage)
                .HasColumnName("size_profile_image");

            builder.Property(i => i.CvImageBits)
                .HasColumnName("cv_image_bits")
                .HasColumnType("VARBINARY(MAX)");

            builder.Property(i => i.TypeCvImage)
                .HasColumnName("type_cv_image")
                .HasMaxLength(10);

            builder.Property(i => i.SizeCvImage)
                .HasColumnName("size_cv_image");

            builder.Property(i => i.FkProfileUser)
                .HasColumnName("fk_profile_user")
                .IsRequired();

            // Unique constraint
            builder.HasIndex(i => i.FkProfileUser)
                .IsUnique();

            // Check constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_type_profile_image",
                "[type_profile_image] IN ('.jpg','.png','.svg','.jpeg','.webp')"
            ));

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_type_cv_image",
                "[type_cv_image] IN ('.jpg','.png','.svg','.jpeg','.webp','.pdf')"
            ));

            // Relationship 1-1
            builder.HasOne(i => i.ProfileUser)
                .WithOne(p => p.Image)
                .HasForeignKey<Images>(i => i.FkProfileUser)
                .HasConstraintName("FK_PROFILE_USER")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}