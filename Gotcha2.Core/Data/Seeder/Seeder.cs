using Gotcha2.Core.Constants;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Data.Seeder
{
    public static class Seeder
    {
        // === Role ===
        private static readonly Guid UserRoleId = new Guid("11111111-1111-1111-1111-111111111111");

        // === Users ===
        // DEV ONLY — passwords for these accounts are committed in plaintext below
        // (Alice!234 / Bob!12345 / Carol!234). Hashes precomputed via PasswordHasher.
        private static readonly Guid AliceId = new Guid("22222222-2222-2222-2222-222222222222");
        private static readonly Guid BobId = new Guid("33333333-3333-3333-3333-333333333333");
        private static readonly Guid CarolId = new Guid("44444444-4444-4444-4444-444444444444");

        private const string AlicePasswordHash = "AQAAAAIAAYagAAAAEOJqkydYrUdO0Jg8/gKHdg/5bMuIglBd3tcLWqMJA1/JAm8ZxuqPcrVCg/lIUItufg==";
        private const string BobPasswordHash = "AQAAAAIAAYagAAAAEHjtRVauX8Vortxb/LpJm/pc3PmzQREasvkCTyu1Kzkq8pPi8Fc83kGAqpldMAMeBQ==";
        private const string CarolPasswordHash = "AQAAAAIAAYagAAAAEORe3416rrXUsb2RutQnzcLqUJishn4nw9DASef9MjoAsDfs+vTVRAZyf6/jOVpqGw==";

        // === Game / Players / Target ring ===
        private static readonly Guid GameId = new Guid("55555555-5555-5555-5555-555555555555");
        private static readonly Guid AlicePlayerId = new Guid("66666666-6666-6666-6666-666666666661");
        private static readonly Guid BobPlayerId = new Guid("66666666-6666-6666-6666-666666666662");
        private static readonly Guid CarolPlayerId = new Guid("66666666-6666-6666-6666-666666666663");
        private static readonly Guid TaAliceBobId = new Guid("77777777-7777-7777-7777-777777777771");
        private static readonly Guid TaBobCarolId = new Guid("77777777-7777-7777-7777-777777777772");
        private static readonly Guid TaCarolAliceId = new Guid("77777777-7777-7777-7777-777777777773");

        // Static timestamp keeps HasData deterministic across builds.
        private static readonly DateTime SeedTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static void Seed(ModelBuilder modelBuilder)
        {
            #region Roles
            IdentityRole<Guid>[] roles = new[]
            {
                new IdentityRole<Guid>
                {
                    Id = UserRoleId,
                    Name = Roles.User,
                    NormalizedName = Roles.User.ToUpperInvariant(),
                    ConcurrencyStamp = UserRoleId.ToString()
                }
            };
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);
            #endregion

            #region Users
            GotchaUser[] users = new[]
            {
                BuildUser(AliceId, "alice@gotcha.dev", "Alice", "Test", Genders.Female, new DateTime(1995, 1, 1), AlicePasswordHash),
                BuildUser(BobId, "bob@gotcha.dev", "Bob", "Test", Genders.Male, new DateTime(1992, 6, 15), BobPasswordHash),
                BuildUser(CarolId, "carol@gotcha.dev", "Carol", "Test", Genders.Female, new DateTime(1998, 11, 20), CarolPasswordHash),
            };
            modelBuilder.Entity<GotchaUser>().HasData(users);
            #endregion

            #region User-role links
            IdentityUserRole<Guid>[] userRoles = new[]
            {
                new IdentityUserRole<Guid> { UserId = AliceId, RoleId = UserRoleId },
                new IdentityUserRole<Guid> { UserId = BobId, RoleId = UserRoleId },
                new IdentityUserRole<Guid> { UserId = CarolId, RoleId = UserRoleId },
            };
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(userRoles);
            #endregion

            #region Game
            modelBuilder.Entity<Game>().HasData(new
            {
                Id = GameId,
                Name = "Demo Game",
                CreationDate = SeedTimestamp,
                StartDate = (DateTime?)SeedTimestamp,
                EndDate = (DateTime?)null,
                HasStarted = true,
                IsFinished = false,
                WinnerId = (Guid?)null,
                CreatorId = AliceId
            });
            #endregion

            #region Players
            modelBuilder.Entity<Player>().HasData(
                new { Id = AlicePlayerId, UserId = AliceId, GameId = GameId, IsAlive = true, Notes = "" },
                new { Id = BobPlayerId, UserId = BobId, GameId = GameId, IsAlive = true, Notes = "" },
                new { Id = CarolPlayerId, UserId = CarolId, GameId = GameId, IsAlive = true, Notes = "" }
            );
            #endregion

            #region Target ring (Alice -> Bob -> Carol -> Alice)
            modelBuilder.Entity<TargetAssignment>().HasData(
                new { Id = TaAliceBobId, HunterId = AlicePlayerId, TargetId = BobPlayerId, GameId = GameId, TargetAssigned = SeedTimestamp, AssignmentFinished = (DateTime?)null, Weapon = (string?)null },
                new { Id = TaBobCarolId, HunterId = BobPlayerId, TargetId = CarolPlayerId, GameId = GameId, TargetAssigned = SeedTimestamp, AssignmentFinished = (DateTime?)null, Weapon = (string?)null },
                new { Id = TaCarolAliceId, HunterId = CarolPlayerId, TargetId = AlicePlayerId, GameId = GameId, TargetAssigned = SeedTimestamp, AssignmentFinished = (DateTime?)null, Weapon = (string?)null }
            );
            #endregion
        }

        private static GotchaUser BuildUser(Guid id, string email, string firstName, string lastName, Genders gender, DateTime birthDate, string passwordHash)
        {
            return new GotchaUser
            {
                Id = id,
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = email,
                NormalizedUserName = email.ToUpperInvariant(),
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                BirthDate = birthDate,
                AccountCreationDate = SeedTimestamp,
                PasswordHash = passwordHash,
                SecurityStamp = id.ToString(),
                ConcurrencyStamp = id.ToString()
            };
        }
    }
}
