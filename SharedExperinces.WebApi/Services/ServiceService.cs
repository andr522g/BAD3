using SharedExperinces.WebApi.DataAccess;
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

        public void Add(Service service)
        {
            _context.Services.Add(service);
        }

        public async Task<Service?> Update(Service service)
        {
            var existingServiceId = await _context.Services.FindAsync(service.ServiceId);

            if(existingServiceId == null)
            {
                return null;
            }


            _context.Services.Update(service);

            return existingServiceId;
        }
    }
}
