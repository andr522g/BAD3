using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;


namespace SharedExperinces.WebApi.Configurations
{
    public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(r => r.RegistrationId);

            builder.HasOne(r => r.Service)
                   .WithMany(s => s.Registrations)
                   .HasForeignKey(r => r.ServiceId);

            builder.HasOne(r => r.Guest)
                   .WithMany(g => g.Registrations)
                   .HasForeignKey(r => r.GuestId);
        }
    }
}
