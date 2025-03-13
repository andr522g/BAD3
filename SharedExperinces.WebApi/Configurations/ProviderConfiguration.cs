using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Configurations
{
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.HasKey(p => p.CVR);

            builder.HasMany(p => p.Services)
                   .WithOne(s => s.Provider)
                   .HasForeignKey(s => s.CVR);
        }
    }
}
