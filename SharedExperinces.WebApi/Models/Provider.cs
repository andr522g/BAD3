using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SharedExperinces.WebApi.Models
{
    public class Provider
    {
        public string Name { get; set; }
        public string Address { get; set; }        
        public string PhoneNumber { get; set; }
		public string PermitFilePath { get; set; }
		public ICollection<Service> Services { get; set; } = new List<Service>();
	}
}
