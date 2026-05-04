# Kills Info

> **OUTDATED — page merged into [Match Result](matchResult.md) on 2026-04-30.** The kill list is now an inline table on Match Result that updates when a player row is tapped. Kept as a historical artifact from the Projectdoel submission.


The Kills Info page is for the Player (logged in, in a finished game). Reached by tapping a player's row on Match Result.

**Shows:**
- The selected player's name
- A list of every player they killed in this game

**Each kill row shows:**
- Victim's name
- Time of kill (formatted local time)

**Tapping a kill row:** expands the row to show extra info (placeholder for Gotcha2 — currently the same fields, no extra data, since `Kill` only has `Moment` + killer + victim + game).

**Notes:**
- Original Gotcha had `Weapon` + `KillMessage` + `Reason` on `Kill`; Gotcha2's `Kill` only has `Moment`, `KillerId`, `VictimId`, `GameId`.
