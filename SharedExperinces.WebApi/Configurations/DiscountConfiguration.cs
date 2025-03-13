using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => new { d.ServiceId, d.GuestCount });  // Composite Key

        builder.HasOne(d => d.Service)
               .WithMany(s => s.Discounts)
               .HasForeignKey(d => d.ServiceId);
    }
}
