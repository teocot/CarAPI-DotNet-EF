using System.ComponentModel.DataAnnotations;

namespace CarAPI.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Make { get; set; }

        [Range(1886, 2100)]
        public int Year { get; set; }

        public string Color { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a person.")]
        public int? PersonId { get; set; }
        public Person? Person { get; set; }

        // Add this to support one-to-one with Purchase
        public Purchase? Purchase { get; set; }
    }
}
