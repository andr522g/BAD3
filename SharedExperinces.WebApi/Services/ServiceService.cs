using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;



namespace SharedExperinces.WebApi.Services
{
    public class ServiceService
    {
        private readonly SharedExperinceContext _context;

        public ServiceService(SharedExperinceContext context)
        {
            _context = context;
        }

        public async Task<string?> AddNewService(CreateAndUpdateServiceDto dto)
        {
            // Check if Provider exists
            var provider = await _context.Providers.FindAsync(dto.PhoneNumber);
            if (provider == null)
                return $"Provider with Id {dto.PhoneNumber} not found.";


			var service = new Service
			{
				Name = dto.Name,
				Description = dto.Description,
				Price = dto.Price,
				ServiceDate = dto.ServiceDate,
				Provider = provider, 
				Discounts = new List<Discount>(), // 
				Registrations = new List<Registration>(), 
				SharedExperiences = new List<SharedExperience>()
			};


			_context.Services.Add(service);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<Service?> UpdateService(int id, CreateAndUpdateServiceDto service)
        {
            var existingServiceId = await _context.Services.FindAsync(id);
            if (existingServiceId == null) return null;


			existingServiceId.Name = service.Name;
			existingServiceId.Description = service.Description;
			existingServiceId.Price = service.Price;
			existingServiceId.ServiceDate = service.ServiceDate;

			_context.Services.Update(existingServiceId);
			await _context.SaveChangesAsync();
            return existingServiceId;
        }

        public async Task<List<Service>?> GetAllServices()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Service?> GetServiceById(int id)
        {
            return await _context.Services.FindAsync(id);
        }
        
        public async Task<bool> DeleteServiceById(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
