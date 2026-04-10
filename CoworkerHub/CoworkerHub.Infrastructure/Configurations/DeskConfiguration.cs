using CoworkerHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoworkerHub.Infrastructure.Configurations
{
    public class DeskConfiguration : IEntityTypeConfiguration<Desk>
    {
        public void Configure(EntityTypeBuilder<Desk> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.PricePerHour).HasColumnType("decimal(18,2)");
            // Чтобы в базе статусы хранились словами ("Available"), а не цифрами (0)
            builder.Property(d => d.Status).HasConversion<string>();

            builder.HasMany(d => d.Booking)
                .WithOne(b => b.Desk)
                .HasForeignKey(b => b.DeskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
