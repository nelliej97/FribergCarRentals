using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Märke")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Modell")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Färg")]
        public string Color { get; set; }

        [Display(Name = "Tillänglig")] 
        public bool IsAvailable { get; set; } = true;
        public string? ImageUrl { get; set; }

    }
}
