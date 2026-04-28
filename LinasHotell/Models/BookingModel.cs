using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinasHotell.Models
{
    [Table("Bookings")]
    [PrimaryKey(nameof(BookingId))]
    public class BookingModel
    {
        public int BookingId { get; set; }

        public int GuestId { get; set; }

        [Required]
        public GuestModel Guest { get; set; } = null!;

        public int RoomId { get; set; }

        [Required]
        public RoomModel Room { get; set; } = null!;

        public int ExtraBeds { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public int Nights => Math.Max(0, (CheckOutDate - CheckInDate).Days);

        public decimal TotalPrice =>
            Nights > 0 && Room != null
                ? Nights * Room.PricePerNight
                : 0m;

    }
}
