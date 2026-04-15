using LinasHotell.Models;
using Microsoft.EntityFrameworkCore;

namespace LinasHotell
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                 : base(options) { }

        public DbSet<RoomModel> Rooms => Set<RoomModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomModel>();
        }
    }
}
