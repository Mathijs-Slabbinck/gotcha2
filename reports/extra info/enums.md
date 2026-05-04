# Enums

Gotcha2 has **2 enums** (down from the original Gotcha's 8).

## Genders
Defined in `Gotcha2.Core/Enums/Genders.cs`. Used by `GotchaUser.Gender`.
- `Male`
- `Female`
- `Other`

## KillRole
Defined in `Gotcha2.Core/Enums/KillRole.cs`. Used by the API/MAUI when describing a kill from a particular player's perspective.
- (values defined in code — placeholder, expand if needed during implementation)

## What's NOT here (compared to original Gotcha)

- ❌ `MaxLobbySize` — no plan-based lobby caps in Gotcha2.
- ❌ `Plan` — no Standard/Premium/Deluxe tiers.
- ❌ `AssignmentStatus` — Gotcha2 derives status from `AssignmentFinished` + linked `Kill` instead of an explicit enum.
- ❌ `LogTypes` / `LogSubTypes` / `StoreItem` / `PlayerStatus` — out of scope.

## Notes

- Mirror these in `Gotcha2.Maui/Enums/` rather than referencing `Gotcha2.Core` from MAUI (the MAUI project has no Core reference — it talks to the API over HTTP).
