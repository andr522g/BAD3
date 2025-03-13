using System;
using System.Collections.Generic;

namespace SharedExperinces.WebApi.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ServiceDate { get; set; }

        public string CVR { get; set; }
        public Provider Provider { get; set; }

        public ICollection<SharedExperienceService> SharedExperienceService { get; set; }
        public ICollection<Discount> Discount { get; set; }
        public ICollection<Registration> Registration { get; set; }
    }
}
