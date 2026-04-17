using LinasHotell.Models;
using Microsoft.EntityFrameworkCore;

namespace LinasHotell
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                 : base(options) { }

        public DbSet<RoomModel> Rooms => Set<RoomModel>();
        public DbSet<GuestModel> Guests => Set<GuestModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomModel>();
            modelBuilder.Entity<GuestModel>();
        }
    }
}
