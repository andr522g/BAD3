namespace SharedExperinces.WebApi.Models
{
    public class Discount
    {
        public int DiscountId { get; set; }
        public int GuestCount { get; set; }
        public double Percentage { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
