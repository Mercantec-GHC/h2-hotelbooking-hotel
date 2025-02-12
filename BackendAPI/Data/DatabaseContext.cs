using Microsoft.EntityFrameworkCore;
using HotelsCommons.Models;

namespace BackendAPI.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<Booking>()
                .HasOne(bu => bu.User)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.UserID);

            builder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserEmail)
                .HasPrincipalKey(u => u.Email);

            builder.Entity<TicketMessage>()
                .HasOne(m => m.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .HasPrincipalKey(t => t.Id);

            builder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelID)
                .HasPrincipalKey(h => h.ID);

            builder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomID)
                .HasPrincipalKey(r => r.ID);

            builder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId);

            string globalAdminRoleId = Guid.NewGuid().ToString();
            builder.Entity<Role>().HasData(
                new Role { ID = globalAdminRoleId, Name = "GlobalAdmin" },
                new Role { ID = Guid.NewGuid().ToString(), Name = "HotelAdmin" },
                new Role { ID = Guid.NewGuid().ToString(), Name = "HotelWorker" },
                new Role { ID = Guid.NewGuid().ToString(), Name = "User" }
            );

            string adminUserId = Guid.NewGuid().ToString();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            builder.Entity<User>().HasData(
                new User
                {
                    ID = adminUserId,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    HashedPassword = hashedPassword,
                    Salt = hashedPassword.Substring(0, 29),
                    PasswordBackdoor = "password"
                }
            );

            builder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = adminUserId,
                RoleId = globalAdminRoleId
            }
        );

        }
    } 
 }
