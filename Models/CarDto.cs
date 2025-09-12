namespace CarAPI.Models
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
    }

}
