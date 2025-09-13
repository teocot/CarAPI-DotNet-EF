namespace CarAPI.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }

        // Foreign key to Person
        public int? PersonId { get; set; }
        public Person? Buyer { get; set; }

        // Foreign key to Car
        public int? CarId { get; set; }
        public Car? Car { get; set; }
    }
}
