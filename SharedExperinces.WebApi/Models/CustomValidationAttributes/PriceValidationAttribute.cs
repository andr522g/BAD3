using System.ComponentModel.DataAnnotations;

namespace SharedExperinces.WebApi.Models.CustomValidationAttributes
{
	public class PriceValidationAttribute : ValidationAttribute
	{

		public PriceValidationAttribute() 
		{

			this.ErrorMessage = "Price must not be negative!";
		}
		public override bool IsValid(object value)
		{
			if (value is int price)
			{
				return price >= 0;
			}
			return false;
		}
	}
}
