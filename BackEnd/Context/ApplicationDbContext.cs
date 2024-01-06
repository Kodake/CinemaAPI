﻿using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().Property(p => p.Nombre).HasMaxLength(50);
        }

        public DbSet<Genero> Generos { get; set; }
    }
}
