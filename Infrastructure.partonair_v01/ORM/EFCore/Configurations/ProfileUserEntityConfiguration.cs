using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
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

            builder.Property(p => p.ShowSelfInDirectory)
                .HasColumnName("show_self_in_directory")
                .HasDefaultValue(true)
                .IsRequired();

            // Seed DiceBear : la valeur SQL par défaut donne un avatar unique aux profils existants.
            builder.Property(p => p.AvatarSeed)
                .HasColumnName("avatar_seed")
                .HasMaxLength(64)
                .HasDefaultValueSql("CONVERT(NVARCHAR(64), NEWID())")
                .IsRequired();

            builder.Property(p => p.AvatarRegenCount)
                .HasColumnName("avatar_regen_count")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.AvatarRegenDate)
                .HasColumnName("avatar_regen_date")
                .HasColumnType("DATE");

            // Besoins du recruteur
            builder.Property(p => p.SoughtPosition)
                .HasColumnName("sought_position")
                .HasMaxLength(150);

            builder.Property(p => p.ContractTypes)
                .HasColumnName("contract_types")
                .HasDefaultValue(ContractTypes.None)
                .IsRequired();

            builder.Property(p => p.Location)
                .HasColumnName("location")
                .HasMaxLength(100);

            builder.Property(p => p.RemotePolicy)
                .HasColumnName("remote_policy")
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.Sector)
                .HasColumnName("sector")
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(p => p.Stars)
                .HasColumnName("stars")
                .HasColumnType("TINYINT");

            // Moyenne / nombre d'avis reçus, recalculés à chaque évaluation (évite un calcul par carte d'annuaire).
            builder.Property(p => p.RatingAverage)
                .HasColumnName("rating_average")
                .HasPrecision(3, 2)
                .HasDefaultValue(0m)
                .IsRequired();

            builder.Property(p => p.RatingCount)
                .HasColumnName("rating_count")
                .HasDefaultValue(0)
                .IsRequired();

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