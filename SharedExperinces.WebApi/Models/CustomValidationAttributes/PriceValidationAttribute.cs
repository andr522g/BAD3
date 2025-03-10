using System.ComponentModel.DataAnnotations;

namespace SharedExperinces.WebApi.Models.CustomValidationAttributes
{
	public class PriceValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is decimal price)
			{
				return price >= 0;
			}
			return false;
		}
	}
}
