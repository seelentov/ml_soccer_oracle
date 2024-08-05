
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Models.Base;
using WebApplication2.Models.Stats;

namespace WebApplication2.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<HeadToHeadBase> HeadToHeadBase { get; set; }
        public DbSet<HeadToHeadInGame> HeadToHeadInGame { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<CheckedGame> CheckedGames { get; set; }

        public string DbPath { get; }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
/*            modelBuilder.Entity<League>()
                .HasMany(e => e.Games)
                .WithOne(e => e.League)
                .IsRequired();*/
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Debug);
        }
    }
}
