using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Configurations
{
	public class SharedExperienceConfiguration : IEntityTypeConfiguration<SharedExperience>
	{
		public void Configure(EntityTypeBuilder<SharedExperience> builder)
		{
			builder.HasKey(se => se.SharedExperienceId);

			builder.HasMany(se => se.Services)
				   .WithMany(s => s.SharedExperiences);

			builder.HasMany(se => se.Guests)
				   .WithMany(g => g.SharedExperiences);
		}
	}
}
