using Backend.Domain.Enums;
using Backend.Domain.Modals;
using Backend.Domain.Modals.Currency;
using Backend.Domain.Modals.Location;
using Backend.Domain.Modals.Milestones;
using Backend.Domain.Modals.PlayerStatistics;
using Backend.Domain.Modals.Recipes;
using Backend.Domain.Modals.Skills;
using Backend.Domain.Modals.Spells;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class GameDbContext : DbContext
    {


        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyType> CurrencyTypes { get; set; }
        public DbSet<Domain.Modals.Location.Location> Locations { get; set; }
        public DbSet<LocationType> LocationTypes { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<MilestoneType> MilestoneTypes { get; set; }
        public DbSet<Statistic> PlayerStatistics { get; set; }
        public DbSet<StatisticType> StatisticTypes { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeType> RecipeTypes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillType> SkillTypes { get; set; }
        public DbSet<SpellStatistic> SpellStatistics { get; set; }
        public DbSet<SpellStatisticType> SpellTypes { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Elixir> ElixirShops { get; set; }
        public DbSet<GameRun> GameRuns { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<RecipeShop> RecipeShops { get; set; }
        public DbSet<Leaderboard> Leaderboard { get; set; }


        public GameDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>(p =>
            {
                p.Property(p => p.ID).ValueGeneratedOnAdd();
                p.HasIndex(p => p.Username);
                p.Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

                p.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
                // Add any other Player-specific configurations here
            });

            modelBuilder.Entity<Leaderboard>(lb =>
            {
                lb.Property(lb => lb.ID).ValueGeneratedOnAdd();
                lb.HasIndex(lb => lb.LocationID);
                lb.Property(lb => lb.Created)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
                lb.HasOne<Player>().WithMany().HasForeignKey(lb => lb.PlayerID).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Currency>(c =>
            {
                c.Property(c => c.ID).ValueGeneratedOnAdd();
                c.HasOne<Player>().WithMany().HasForeignKey(c => c.PlayerID).OnDelete(DeleteBehavior.Cascade);
                c.HasOne<CurrencyType>().WithMany().HasForeignKey(c => c.TypeID).OnDelete(DeleteBehavior.Cascade);
                c.HasIndex(c => new { c.PlayerID, c.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Domain.Modals.Location.Location>(l =>
            {
                l.Property(l => l.ID).ValueGeneratedOnAdd();
                l.HasOne<Player>().WithMany().HasForeignKey(l => l.PlayerID).OnDelete(DeleteBehavior.Cascade);
                l.HasOne<LocationType>().WithMany().HasForeignKey(l => l.TypeID).OnDelete(DeleteBehavior.Cascade); ;
                l.HasIndex(l => new { l.PlayerID, l.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Milestone>(m =>
            {
                m.Property(m => m.ID).ValueGeneratedOnAdd();
                m.HasOne<Player>().WithMany().HasForeignKey(m => m.PlayerID).OnDelete(DeleteBehavior.Cascade);
                m.HasOne<MilestoneType>().WithMany().HasForeignKey(m => m.TypeID).OnDelete(DeleteBehavior.Cascade);
                m.HasIndex(m => new { m.PlayerID, m.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Statistic>(s =>
            {
                s.Property(s => s.ID).ValueGeneratedOnAdd();
                s.HasOne<Player>().WithMany().HasForeignKey(s => s.PlayerID).OnDelete(DeleteBehavior.Cascade);
                s.HasOne<StatisticType>().WithMany().HasForeignKey(s => s.TypeID).OnDelete(DeleteBehavior.Cascade);
                s.HasIndex(s => new { s.PlayerID, s.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Recipe>(r =>
            {
                r.Property(r => r.ID).ValueGeneratedOnAdd();
                r.HasOne<Player>().WithMany().HasForeignKey(r => r.PlayerID).OnDelete(DeleteBehavior.Cascade);
                r.HasOne<RecipeType>().WithMany().HasForeignKey(r => r.TypeID).OnDelete(DeleteBehavior.Cascade);
                r.HasIndex(r => new { r.PlayerID, r.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Skill>(s =>
            {
                s.Property(s => s.ID).ValueGeneratedOnAdd();
                s.HasOne<Player>().WithMany().HasForeignKey(s => s.PlayerID).OnDelete(DeleteBehavior.Cascade);
                s.HasOne<SkillType>().WithMany().HasForeignKey(s => s.TypeID).OnDelete(DeleteBehavior.Cascade);
                s.HasIndex(s => new { s.PlayerID, s.TypeID }).IsUnique();
            });

            modelBuilder.Entity<SpellStatistic>(ss =>
            {
                ss.Property(ss => ss.ID).ValueGeneratedOnAdd();
                ss.HasOne<Player>().WithMany().HasForeignKey(ss => ss.PlayerID).OnDelete(DeleteBehavior.Cascade);
                ss.HasOne<SpellStatisticType>().WithMany().HasForeignKey(ss => ss.TypeID);
                ss.HasIndex(ss => new { ss.PlayerID, ss.TypeID }).IsUnique();
            });

            modelBuilder.Entity<Elixir>(es =>
            {
                es.Property(es => es.ID).ValueGeneratedOnAdd();
                es.HasOne<Player>().WithMany().HasForeignKey(es => es.PlayerID).OnDelete(DeleteBehavior.Cascade);
                es.HasIndex(es => new { es.PlayerID, es.ElixirID });
            });

            modelBuilder.Entity<GameRun>(gr =>
            {
                gr.Property(gr => gr.GameRunID).ValueGeneratedOnAdd();
                gr.HasOne<Player>().WithMany().HasForeignKey(gr => gr.PlayerID).OnDelete(DeleteBehavior.Cascade);
                gr.HasIndex(gr => gr.PlayerID);
                gr.Property(gr => gr.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
                
            });

            modelBuilder.Entity<History>(h =>
            {
                h.Property(h => h.ID).ValueGeneratedOnAdd();
                h.HasOne<Player>().WithMany().HasForeignKey(h => h.PlayerID).OnDelete(DeleteBehavior.Cascade);
                h.HasOne<Domain.Modals.Location.Location>().WithMany().HasForeignKey(h => h.LocationID).OnDelete(DeleteBehavior.Cascade);
                h.HasIndex(h => h.PlayerID);
                h.Property(p => p.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RecipeShop>(rs =>
            {
                rs.Property(rs => rs.ID).ValueGeneratedOnAdd();
                rs.HasOne<Player>().WithMany().HasForeignKey(rs => rs.PlayerID).OnDelete(DeleteBehavior.Cascade);
                rs.HasIndex(rs => rs.PlayerID).IsUnique();
            });

         
        }


    }
}
