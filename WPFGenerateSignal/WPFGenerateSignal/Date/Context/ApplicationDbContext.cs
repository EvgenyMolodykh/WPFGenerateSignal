using Microsoft.EntityFrameworkCore;
using WPFGenerateSignal.Date.Entities;

namespace WPFGenerateSignal.Date.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SignalEntity> Signals { get; set; }
        public DbSet<SignalPointEntity> SignalPoints { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=signals.db");
        }
    }
}