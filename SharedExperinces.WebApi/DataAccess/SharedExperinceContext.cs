using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.DataAccess
{
	public class SharedExperinceContext : DbContext
	{		
		public DbSet<Discount> Discounts { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<SharedExperience> SharedExperiences { get; set; }
        public DbSet<SharedExperienceGuest> SharedExperienceGuest { get; set; }
        public DbSet<SharedExperienceService> SharedExperienceService { get; set; }

        public SharedExperinceContext(DbContextOptions<SharedExperinceContext> options) : base(options)
		{

		}

		public static void Seed(SharedExperinceContext context)
		{
			context.Database.EnsureCreated();

			context.Providers.Add(new Provider
			{
				CVR = "12345678",
				Name = "Provider 1",
				Address = "Provider 1 Address",
				PhoneNumber = "12345678"
			});
			context.Providers.Add(new Provider
			{
				CVR = "87654321",
				Name = "Provider 2",
				Address = "Provider 2 Address",
				PhoneNumber = "87654321"
			});
			context.SaveChanges();
		}
	}
}
