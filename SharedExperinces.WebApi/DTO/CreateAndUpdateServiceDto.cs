using SharedExperinces.WebApi.Models.CustomValidationAttributes;

namespace SharedExperinces.WebApi.DTO
{
	public class CreateAndUpdateServiceDto
	{

		public string Name { get; set; }
		public string Description { get; set; }
		[PriceValidation(ErrorMessage = "Price must be non-negative!")]
		public int Price { get; set; }
		public DateTime ServiceDate { get; set; }
		public string PhoneNumber { get; set; } 
	}
}
