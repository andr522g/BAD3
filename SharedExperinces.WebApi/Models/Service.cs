using SharedExperinces.WebApi.Models.CustomValidationAttributes;
using System;
using System.Collections.Generic;

namespace SharedExperinces.WebApi.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [PriceValidation]
        public decimal Price { get; set; }
        public DateTime ServiceDate { get; set; }

        public string CVR { get; set; }
        public Provider Provider { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public ICollection<Registration> Registrations { get; set; }
        public ICollection<SharedExperience> SharedExperiences { get; set; }
    }
}
