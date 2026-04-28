using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinasHotell.Models
{
    [Table("Guests")]
    [PrimaryKey(nameof(GuestId))]
    public class GuestModel
    {
        public int GuestId { get; set; }

        [Required, MaxLength(120)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(120)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(160)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        public int? PhoneNumber { get; set; }

        public bool IsCheckedIn { get; set; }

        public List<BookingModel> Bookings { get; set; } = new();

        public override string ToString()
        {
            return 
                $"{FirstName} {LastName}\n" +
                $"{Email}\n" +
                $"{PhoneNumber}";
        }
    }
}
