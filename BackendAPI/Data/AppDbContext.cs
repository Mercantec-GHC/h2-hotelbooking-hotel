using HotelsCommons.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace BackendAPI.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Ticket> Tickets;
        public DbSet<TicketMessage> TicketMessages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Property(u => u.FirstName)
                .HasMaxLength(32)
                .IsRequired();

            builder.Entity<User>()
                .Property(u => u.LastName)
                .HasMaxLength(32)
                .IsRequired();

            builder.HasDefaultSchema("identity");

            builder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserEmail)
                .HasPrincipalKey(u => u.Email);

            builder.Entity<Ticket>()
                .HasMany(t => t.Messages)
                .WithOne(m => m.Ticket)
                .HasForeignKey(m => m.TicketId);
        }
    }
}
