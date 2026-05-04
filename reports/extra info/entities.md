# Entities

Gotcha2 has **5 entities** (down from the original Gotcha's 9). All keys are `Guid`. FKs are `Restrict` except `Player → Game` (Cascade) and `TargetAssignment → Kill` (SetNull, shadow `KillId`).

## GotchaUser
- `IdentityUser<Guid>` (ASP.NET Identity user with a Guid key)
- `string FirstName`
- `string LastName`
- `required string UserName` (inherited from Identity, marked `required` to enforce non-null)
- `required string Email`
- `bool HasProfileImage`
- `Genders Gender`
- `DateTime BirthDate`
- `DateTime AccountCreationDate` (init-only)
- `bool IsDeleted`
- `ICollection<Player> PlayerAccounts` — one user can have many `Player` rows (one per game they joined)

## Game
- `Guid Id`
- `string Name = "New game"`
- `DateTime CreationDate` (init-only)
- `DateTime? StartDate`
- `DateTime? EndDate`
- `bool HasStarted`
- `bool IsFinished`
- `Guid? WinnerId`
- `Guid CreatorId`
- `ICollection<Player> Players`
- `ICollection<Kill> Kills`

## Player
Join row between `GotchaUser` and `Game`.
- `Guid Id`
- `Guid UserId` + `GotchaUser User`
- `Guid GameId` + `Game Game`
- `bool IsAlive = true`
- `string Notes` (free text)
- `ICollection<TargetAssignment> TargetAssignments` (as Hunter — Target side has no inverse)

## TargetAssignment
- `Guid Id`
- `Guid HunterId` + `Player Hunter`
- `Guid TargetId` + `Player Target`
- `Guid GameId` + `Game Game`
- `DateTime TargetAssigned`
- `DateTime? AssignmentFinished`
- `string? Weapon` (kept as nullable for future use; currently unused since no `Rules` entity)
- `Kill? Kill` (shadow `KillId`, `OnDelete: SetNull`)

## Kill
- `Guid Id`
- `Guid GameId` + `Game Game`
- `Guid KillerId` + `Player Killer`
- `Guid VictimId` + `Player Victim`
- `DateTime Moment` (init-only, defaults to `UtcNow`)

## What's NOT here (compared to original Gotcha)

- ❌ `Rules` — no per-game settings (Assassin, Chaos, custom kill methods, timed kills, …)
- ❌ `VipSettings` — no plans, no in-app purchases
- ❌ `ProfileImage` (separate entity) — Gotcha2 stores image as a flag on `GotchaUser` (`HasProfileImage`), with bytes uploaded to the API, not as a separate entity
- ❌ `Log` / `Attacker` / `Error` / `Warning` / `HackAttempt` — no custom logging entities

## Notes

- The kill flow is: Player taps "I got my target" → API creates a `Kill`, sets victim's `Player.IsAlive = false`, links the victim's old `TargetAssignment.Kill` to the new `Kill`, and creates a new `TargetAssignment` for the killer pointing at the victim's old target.
- Game ends when only one `Player` has `IsAlive = true`. The API sets `Game.IsFinished = true`, `Game.EndDate = UtcNow`, `Game.WinnerId = lastAlivePlayer.UserId`.
