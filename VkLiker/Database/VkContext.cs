using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class VkContext : DbContext
    {
        public VkContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VkCity>();
            modelBuilder.Entity<VkUser>().HasOne(u => u.VkCity);
            var likeBuilder = modelBuilder.Entity<VkLike>();
            likeBuilder.HasOne(l => l.CurrentUser);
            likeBuilder.HasOne(l => l.Previous );

        }
    }
}
