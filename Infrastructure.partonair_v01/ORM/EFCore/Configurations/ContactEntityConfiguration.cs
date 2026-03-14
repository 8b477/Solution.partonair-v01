using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class ContactEntityConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts");

            // Primary Key
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(c => c.ContactName)
                .HasColumnName("contact_name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.ContactMail)
                .HasColumnName("contact_mail")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(c => c.IsFriendly)
                .HasColumnName("is_friendly");

            builder.Property(c => c.AcceptedAt)
                .HasColumnName("accepted_at")
                .HasColumnType("DATETIME");

            builder.Property(c => c.BlockedAt)
                .HasColumnName("blocked_at")
                .HasColumnType("DATETIME");

            builder.Property(c => c.IsBlocked)
                .HasColumnName("is_blocked");

            builder.Property(c => c.ContactStatus)
                   .HasColumnName("contact_status")
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .HasDefaultValue(StatusContact.Pending);

            builder.Property(c => c.ContactReceiverId)
                .HasColumnName("fk_contact_receiver")
                .IsRequired();

            builder.Property(c => c.ContactSenderId)
                .HasColumnName("fk_contact_sender")
                .IsRequired();

            // Unique constraint (Sender + Receiver = PK composite)
            builder.HasIndex(c => new { c.ContactSenderId, c.ContactReceiverId })
                .IsUnique()
                .HasDatabaseName("UQ_Contacts_Sender_Receiver");

            // Check constraints
            builder.ToTable(t => t.HasCheckConstraint("CK_Contacts_Status_Valid", "[contact_status] IN ('Pending','Accepted','Blocked','Refused')"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Contacts_NoSelf", "[fk_contact_sender] <> [fk_contact_receiver]"));

            // Sender relationship
            builder.HasOne(c => c.ContactSender)
                .WithMany(u => u.ContactsSent)
                .HasForeignKey(c => c.ContactSenderId)
                .HasConstraintName("FK_CONTACT_SENDER")
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver relationship
            builder.HasOne(c => c.ContactReceiver)
                .WithMany(u => u.ContactsReceived)
                .HasForeignKey(c => c.ContactReceiverId)
                .HasConstraintName("FK_CONTACT_RECEIVER")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}