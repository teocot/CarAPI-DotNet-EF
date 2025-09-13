namespace CarAPI.Models
{
    public class PurchaseViewModel
    {
        public int purchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PersonId { get; set; }
        public int CarId { get; set; }
    }
}
