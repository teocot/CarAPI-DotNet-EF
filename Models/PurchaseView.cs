using System.ComponentModel.DataAnnotations;
using System;

namespace CarAPI.Models
{

    public class PurchaseViewModel
    {
        public int purchaseId { get; set; }

        [Required(ErrorMessage = "Purchase date is required")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Buyer is required")]
        public int? PersonId { get; set; }

        [Required(ErrorMessage = "Car is required")]
        public int? CarId { get; set; }
    }

}
