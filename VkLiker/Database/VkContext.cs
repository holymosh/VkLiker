﻿using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public sealed class VkContext : DbContext
    {
        public DbSet<VkCity> VkCities { get; private set; }
        public VkContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            VkCities = Set<VkCity>();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VkCity>();
            modelBuilder.Entity<VkUser>().HasOne(u => u.VkCity);
            var likeBuilder = modelBuilder.Entity<VkLike>();
            likeBuilder.HasOne(l => l.CurrentUser);

        }
    }
}
