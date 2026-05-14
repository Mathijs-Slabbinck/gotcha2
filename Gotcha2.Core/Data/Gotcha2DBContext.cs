using Gotcha2.Core.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Data
{
    public class Gotcha2DBContext : IdentityDbContext<GotchaUser, IdentityRole<Guid>, Guid>
    {
        public Gotcha2DBContext(DbContextOptions<Gotcha2DBContext> options) : base(options) { }

        public DbSet<Game> Games => Set<Game>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<TargetAssignment> TargetAssignments => Set<TargetAssignment>();
        public DbSet<Kill> Kills => Set<Kill>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            Seeder.Seeder.Seed(builder);

            // Player -> GotchaUser (one user can have many player accounts, one per game)
            builder.Entity<Player>()
                .HasOne(p => p.User)
                .WithMany(u => u.PlayerAccounts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Player -> Game
            builder.Entity<Player>()
                .HasOne(p => p.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // TargetAssignment -> Hunter (Player.TargetAssignments is the inverse)
            builder.Entity<TargetAssignment>()
                .HasOne(ta => ta.Hunter)
                .WithMany(p => p.TargetAssignments)
                .HasForeignKey(ta => ta.HunterId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetAssignment -> Target (no inverse collection on Player)
            builder.Entity<TargetAssignment>()
                .HasOne(ta => ta.Target)
                .WithMany()
                .HasForeignKey(ta => ta.TargetId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetAssignment -> Game (no inverse collection on Game)
            builder.Entity<TargetAssignment>()
                .HasOne(ta => ta.Game)
                .WithMany()
                .HasForeignKey(ta => ta.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            // TargetAssignment -> Kill (optional 1:1, shadow FK "KillId" on TargetAssignment)
            builder.Entity<TargetAssignment>()
                .HasOne(ta => ta.Kill)
                .WithOne()
                .HasForeignKey<TargetAssignment>("KillId")
                .OnDelete(DeleteBehavior.SetNull);

            // Kill -> Game
            builder.Entity<Kill>()
                .HasOne(k => k.Game)
                .WithMany(g => g.Kills)
                .HasForeignKey(k => k.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kill -> Killer / Victim (no inverse collections on Player)
            builder.Entity<Kill>()
                .HasOne(k => k.Killer)
                .WithMany()
                .HasForeignKey(k => k.KillerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Kill>()
                .HasOne(k => k.Victim)
                .WithMany()
                .HasForeignKey(k => k.VictimId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
