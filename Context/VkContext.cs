using Microsoft.EntityFrameworkCore;
using VkBot.Models;

namespace VkBot.Context
{
    public class VkContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }
        public VkContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VkBot;Trusted_Connection=True;");
        }
    }
}
