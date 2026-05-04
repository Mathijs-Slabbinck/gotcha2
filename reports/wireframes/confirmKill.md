# Confirm Kill

The Confirm Kill page is for the Player (logged in, in an active game, alive).

**The page has two main actions:**
- **I got my target** — sends a kill claim to the API. The kill becomes "pending" until the target confirms (or until the timer expires — out of scope for Gotcha2).
- **I was killed** — marks the player as dead. (Self-report path; the simpler flow.)

**For Gotcha2's stripped scope:** a kill is recorded immediately when either party reports it. No two-party confirmation timer (the original had a `KillConfirmationTimer` field that's gone in Gotcha2).

**Confirmation flow:**
1. Player taps "I got my target".
2. App calls `POST /api/games/{id}/kills` with the kill claim.
3. API moves the target to `IsAlive = false`, creates the `Kill`, and re-assigns the killer's `TargetAssignment` to point at the victim's old target.
4. Page navigates back to [Player Home](playerHome.md) (showing the new target).

**Notes:**
- No camera-as-proof in Gotcha2 — Camera is scoped to profile pictures only.
- No geolocation tagging.
