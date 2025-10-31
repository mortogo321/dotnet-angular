using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoard.Core.Entities;

namespace TaskBoard.Infrastructure.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Position)
            .IsRequired();

        builder.Property(c => c.Priority)
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .HasMaxLength(20);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(c => new { c.ListId, c.Position });
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.DueDate);
        builder.HasIndex(c => c.Priority);
    }
}
