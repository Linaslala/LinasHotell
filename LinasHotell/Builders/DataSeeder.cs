using System;
using System.Collections.Generic;
using System.Text;
using LinasHotell.Models;

namespace LinasHotell.Builders
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            var rooms = new List<RoomModel>
            {
                new() { RoomNumber = 101, RoomType = RoomTypeEnums.Single, PricePerNight = 750, ExtraBedsAllowed = 0, RoomIsActive = true },
                new() { RoomNumber = 102, RoomType = RoomTypeEnums.Double, PricePerNight = 1100, ExtraBedsAllowed = 1, RoomIsActive = true },
                new() { RoomNumber = 201, RoomType = RoomTypeEnums.Double, PricePerNight = 1200, ExtraBedsAllowed = 2, RoomIsActive = true },
                new() { RoomNumber = 301, RoomType = RoomTypeEnums.Suite, PricePerNight = 2200, ExtraBedsAllowed = 2, RoomIsActive = true }
            };

            db.Rooms.AddRange(rooms);
            db.SaveChanges();
        }
    }
}
