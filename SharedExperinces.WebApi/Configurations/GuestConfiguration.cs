using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;


namespace SharedExperinces.WebApi.Configurations
{
    public class GuestConfiguration : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> builder)
        {
            builder.HasKey(g => g.GuestId);

            builder.HasMany(g => g.Registrations)
                   .WithOne(r => r.Guest)
                   .HasForeignKey(r => r.GuestId);

            builder.HasMany(g => g.SharedExperiences)
                   .WithMany(se => se.Guests)


        }

    }
}
