namespace CarAPI.Models
{
    namespace CarAPI.Models
    {
        public class PurchaseDto
        {
            public int Id { get; set; }
            public DateTime PurchaseDate { get; set; }

            public int PersonId { get; set; }
            public string PersonName { get; set; }

            public int CarId { get; set; }
            public string CarModel { get; set; }
        }
    }
}
