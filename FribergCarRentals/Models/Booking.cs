using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FribergCarRentals.Models
{
    public class Booking
    {
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public IdentityUser? User { get; set; }

        public int Id { get; set; }

        [Display(Name = "Bil")]
        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car? Car { get; set; }

        [Display(Name = "Kund")]
        public int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
    }
}
