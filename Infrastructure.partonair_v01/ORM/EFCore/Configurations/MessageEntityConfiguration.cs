using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.partonair_v01.ORM.EFCore.Configurations
{
    public class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages", table =>
            {
                table.HasCheckConstraint("CK_Messages_NoSelf", "[sender_id] <> [receiver_id]");
                table.HasCheckConstraint("CK_Messages_Content_NotEmpty", "LEN(LTRIM(RTRIM([content]))) > 0");
            });

            builder.HasKey(message => message.Id);
            builder.Property(message => message.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(message => message.SenderId)
                .HasColumnName("sender_id")
                .IsRequired();
            builder.Property(message => message.ReceiverId)
                .HasColumnName("receiver_id")
                .IsRequired();
            builder.Property(message => message.Content)
                .HasColumnName("content")
                .HasMaxLength(4000)
                .IsRequired();
            builder.Property(message => message.SentAt)
                .HasColumnName("sent_at")
                .HasColumnType("DATETIME2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(message => message.ReadAt)
                .HasColumnName("read_at")
                .HasColumnType("DATETIME2");

            builder.HasIndex(message => new { message.SenderId, message.ReceiverId, message.SentAt })
                .HasDatabaseName("IX_Messages_Conversation");
            builder.HasIndex(message => new { message.ReceiverId, message.SenderId, message.SentAt })
                .HasDatabaseName("IX_Messages_Conversation_Reverse");
            builder.HasIndex(message => new { message.ReceiverId, message.ReadAt })
                .HasDatabaseName("IX_Messages_Unread");

            builder.HasOne(message => message.Sender)
                .WithMany(user => user.MessagesSent)
                .HasForeignKey(message => message.SenderId)
                .HasConstraintName("FK_MESSAGES_SENDER")
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(message => message.Receiver)
                .WithMany(user => user.MessagesReceived)
                .HasForeignKey(message => message.ReceiverId)
                .HasConstraintName("FK_MESSAGES_RECEIVER")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
