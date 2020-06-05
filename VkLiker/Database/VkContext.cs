using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public sealed class VkContext : DbContext
    {
        public DbSet<City> Cities { get; }
        public DbSet<ApplicationInitOptions> InitOptions { get; }
        public DbSet<Region> Regions { get; }

        public VkContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            Cities = Set<City>();
            Regions = Set<Region>();
            InitOptions = Set<ApplicationInitOptions>();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasOne(c => c.Region);
            modelBuilder.Entity<User>().HasOne(u => u.City);
            var likeBuilder = modelBuilder.Entity<VkLike>();
            likeBuilder.HasOne(l => l.CurrentUser);
            var regionBuilder = modelBuilder.Entity<Region>();
        }
    }
}
