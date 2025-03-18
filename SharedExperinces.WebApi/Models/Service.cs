using SharedExperinces.WebApi.Models.CustomValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedExperinces.WebApi.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [PriceValidation]
        public int Price { get; set; }
        public DateTime ServiceDate { get; set; }


        [ForeignKey("Provider")]
		public string PhoneNumber { get; set; }
        public Provider Provider { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<SharedExperience> SharedExperiences { get; set; }

        public Service()
		{
			Discounts = new List<Discount>();
			Registrations = new List<Registration>();
			SharedExperiences = new List<SharedExperience>();
		}
	}
}
