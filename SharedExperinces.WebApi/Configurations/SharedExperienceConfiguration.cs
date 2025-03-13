using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Configurations
{
	public class SharedExperienceConfiguration : IEntityTypeConfiguration<SharedExperience>
	{
		public void Configure(EntityTypeBuilder<SharedExperience> builder)
		{

			builder
		   .HasMany(se => se.Services)
		   .WithMany(s => s.SharedExperiences) 
		   .UsingEntity<Dictionary<string, object>>( 
			   "SharedExperienceService",  
			   j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"),
			   j => j.HasOne<SharedExperience>().WithMany().HasForeignKey("SharedExperienceId")); 
		}
	}
}
