using LinasHotell.GlobalUtilities.SoftDelete;
using LinasHotell.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinasHotell.Builders
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //public DbSet<GuestModel> Guests => Set<GuestModel>();
        public DbSet<RoomModel> Rooms => Set<RoomModel>();
        //public DbSet<BookingModel> Bookings => Set<BookingModel>();
        //public DbSet<InvoiceModel> Invoices => Set<InvoiceModel>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
        .AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<GuestModel>()
            //   .HasIndex(g => g.Email)
            //   .IsUnique();

            modelBuilder.Entity<RoomModel>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            modelBuilder.Entity<RoomModel>()
                .Property(r => r.PricePerNight)
                .HasColumnType("decimal(18,2)");

            //modelBuilder.Entity<BookingModel>()
            //    .HasOne(b => b.Guest)
            //    .WithMany(g => g.Bookings)
            //    .HasForeignKey(b => b.GuestId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BookingModel>()
            //    .HasOne(b => b.Room)
            //    .WithMany(r => r.Bookings)
            //    .HasForeignKey(b => b.RoomId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<BookingModel>()
            //    .HasOne(b => b.Invoice)
            //    .WithOne(i => i.Booking)
            //    .HasForeignKey<InvoiceModel>(i => i.BookingId)
            //    .IsRequired()
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<InvoiceModel>()
            //    .Property(i => i.Amount)
            //    .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
