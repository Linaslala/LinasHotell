using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LinasHotell.GlobalUtilities.SoftDelete;

namespace LinasHotell.Models 
{
    [Table("Rooms")]
    [PrimaryKey(nameof(RoomId))]
    public class RoomModel : ISoftDelete
    {
        public int RoomId { get; set; }

        [Required, MaxLength(20)]
        public int RoomNumber { get; set; } = 0;

        [Required]
        public RoomTypeEnums RoomType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(0,2)]
        public int ExtraBedsAllowed { get; set; } = 0;

        [Required]
        public bool IsDeleted { get; set; }

        //public List<BookingModel> Bookings { get; set; } = new();
    }
}
