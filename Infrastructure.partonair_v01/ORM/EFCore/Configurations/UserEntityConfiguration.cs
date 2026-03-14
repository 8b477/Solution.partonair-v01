using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.partonair_v01.Enums;


namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(u => u.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(u => u.Mail)
                .HasColumnName("mail")
                .HasMaxLength(250)
                .IsRequired();

            builder.HasIndex(u => u.Mail)
                .IsUnique();

            builder.Property(u => u.PasswordHashed)
                .HasColumnName("password_hashed")
                .HasColumnType("VARCHAR(MAX)")
                .IsRequired();

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasColumnName("role")
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(Roles.Visitor);

            builder.Property(u => u.UserCreatedAt)
                .HasColumnName("user_created_at")
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(u => u.UserUpdatedAt)
                .HasColumnName("user_updated_at")
                .HasColumnType("DATETIME");

            builder.Property(u => u.LastConnection)
                .HasColumnName("last_connection")
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            builder.Property(u => u.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(false)
                .IsRequired();

            // Check constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_Users_Role_Valid", "role IN ('Visitor', 'Admin', 'Moderator')"));
        }
    }
}