using CoworkerHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoworkerHub.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(b => b.Status).HasConversion<string>();
        }
    }
}
