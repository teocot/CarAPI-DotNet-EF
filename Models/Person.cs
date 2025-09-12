using System.ComponentModel.DataAnnotations;

namespace CarAPI.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Car>? Cars { get; set; }

        // One person can have many purchases
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    }
}
