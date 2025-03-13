using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Configurations
{
	public class ServiceConfiguration : IEntityTypeConfiguration<Service>
	{
		public void Configure(EntityTypeBuilder<Service> builder)
		{

			builder.HasKey(s => s.ServiceId);

			builder
		   .HasMany(s => s.Registrations)
		   .WithOne(r => r.Service)
		   .HasForeignKey(r => r.ServiceId)
		   .OnDelete(DeleteBehavior.Cascade); 
		}
	}
}
