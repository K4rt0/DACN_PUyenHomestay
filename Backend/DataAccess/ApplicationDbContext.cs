using Backend.Models;
using Backend.Models.VietnamProvinces;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchContact> BranchContacts { get; set; }
        public DbSet<BranchContactDetail> BranchContactDetails { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilitiesRoom> FacilitiesRooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationDetail> ReservationDetails { get; set; }

        // Vietnam provinces
        public DbSet<Province> Provinces { get; set; }  
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
    }
}