using Microsoft.EntityFrameworkCore;
using StakeholdersService.Domain;
using System;
namespace StakeholdersService.Database
{
    public class StakeholdersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("stakeholders");

            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.HasAnnotation("Relational:Collation", null);


        }



    }

}
