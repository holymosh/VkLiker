using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public sealed class VkContext : DbContext
    {
        public DbSet<RegionPart> Cities { get; }
        public DbSet<ApplicationInitOptions> InitOptions { get; }
        public DbSet<VkRegion> Regions { get; }

        public VkContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            Cities = Set<RegionPart>();
            Regions = Set<VkRegion>();
            InitOptions = Set<ApplicationInitOptions>();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegionPart>().HasOne(c => c.VkRegion);
            var userBuilder = modelBuilder.Entity<User>();
            userBuilder.HasOne(u => u.RegionPart);
            userBuilder.HasMany(u => u.Friends).WithOne(u => u.Previous);
            var likeBuilder = modelBuilder.Entity<VkLike>();
            likeBuilder.HasOne(l => l.CurrentUser);
            var regionBuilder = modelBuilder.Entity<VkRegion>();
        }
    }
}
