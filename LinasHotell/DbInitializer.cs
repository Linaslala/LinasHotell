using LinasHotell.Models;
using Microsoft.EntityFrameworkCore;

namespace LinasHotell
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            if (db.Rooms.Any() || db.Guests.Any() || db.Bookings.Any())
                return;

            var rooms = new List<RoomModel>
            {
                new() { RoomNumber = 101, RoomType = RoomTypeEnums.Single, ExtraBedsAllowed = 0, PricePerNight = 750 },
                new() { RoomNumber = 102, RoomType = RoomTypeEnums.Double, ExtraBedsAllowed = 1, PricePerNight = 1100 },
                new() { RoomNumber = 201, RoomType = RoomTypeEnums.Double, ExtraBedsAllowed = 1, PricePerNight = 1200 },
                new() { RoomNumber = 301, RoomType = RoomTypeEnums.Suite, ExtraBedsAllowed = 2,  PricePerNight = 2200 }
            };

            db.Rooms.AddRange(rooms);
            db.SaveChanges();

            var guests = new List<GuestModel>
            {
                new() { Email = "lina.sk.mail@gmail.com", FirstName = "Lina", LastName = "Samuelsson", PhoneNumber = 0701111111, IsCheckedIn = false },
                new() { Email = "tomas@wejskog.com", FirstName = "Tomas", LastName = "Wejskog", PhoneNumber = 0701111112, IsCheckedIn = false},
                new() { Email = "levi@samuelsson.com", FirstName = "Levi", LastName = "Samuelsson", PhoneNumber = 0701111113, IsCheckedIn = false},
                new() { Email = "tor@wejskog.com", FirstName = "Tor", LastName = "Wejskog", PhoneNumber = 0701111114, IsCheckedIn = true},
            };

            db.Guests.AddRange(guests);
            db.SaveChanges();

            var today = DateTime.Today;

            var bookings = new List<BookingModel>
            {
                new()
                {
                    GuestId = guests[0].GuestId,
                    RoomId  = rooms[0].RoomId,
                    CheckInDate = today.AddDays(1),
                    CheckOutDate = today.AddDays(3)
                },
                new()
                {
                    GuestId = guests[1].GuestId,
                    RoomId  = rooms[1].RoomId,
                    CheckInDate = today.AddDays(2),
                    CheckOutDate = today.AddDays(5)
                },
                new()
                {
                    GuestId = guests[2].GuestId,
                    RoomId  = rooms[2].RoomId,
                    CheckInDate = today.AddDays(5),
                    CheckOutDate = today.AddDays(8)
                },
                new()
                {
                    GuestId = guests[3].GuestId,
                    RoomId  = rooms[3].RoomId,
                    CheckInDate = today.AddDays(0),
                    CheckOutDate = today.AddDays(14)
                }
            };

            db.Bookings.AddRange(bookings);
            db.SaveChanges();

            await db.SaveChangesAsync();
        }
    }
}
