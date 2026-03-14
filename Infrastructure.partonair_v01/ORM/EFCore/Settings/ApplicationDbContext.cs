using Domain.partonair_v01.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.partonair_v01.ORM.EFCore.Settings
{
    public class ApplicationDbContext : DbContext
    {

        // Get options from DI (parent class)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {  }


        // Set entities
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ProfileUser> ProfileUsers { get; set; } = null!;
        public DbSet<Images> Images { get; set; } = null!;
        public DbSet<Announcement> Annoucements { get; set; } = null!;
        public DbSet<Evaluation> Evaluations { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;


        // Apply configs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
