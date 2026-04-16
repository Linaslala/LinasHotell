using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinasHotell.Models 
{
    [Table("Rooms")]
    [PrimaryKey(nameof(RoomId))]
    public class RoomModel
    {
        public int RoomId { get; set; }

        [Required, MaxLength(20)]
        public int RoomNumber { get; set; } 

        [Required]
        public RoomTypeEnums RoomType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(0,2)]
        public int ExtraBedsAllowed { get; set; } 

        public bool IsBookable { get; set; }

        //public List<BookingModel> Bookings { get; set; } = new();

        public override string ToString()
        {
            return
                $"Rumsnummer: {RoomNumber}, " +
                $"RumsTyp: {RoomType}, " +
                $"Pris per natt: {PricePerNight}, " +
                $"Tillåtet antal extrasängar: {ExtraBedsAllowed}, "; /*+*/
            //$"Aktuella bokningar: {List<Bookings>}";
        }
    }
}

public enum RoomTypeEnums
{
    Single = 1,
    Double = 2,
    Suite = 3
}
