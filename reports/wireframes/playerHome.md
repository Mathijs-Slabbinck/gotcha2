# Player Home

The Player Home page is for the Player (logged in user, in a specific active game). Reached by tapping an active game on the Games list — the page receives the `gameId` as a query parameter (`?gameId=...`) and loads everything scoped to that game.

## Sections (top to bottom)

### 1. Player header
- Profile image
- Name + username
- Game name (so the user knows which game this view is for)

### 2. Current target (when alive) / Killed-by (when eliminated)

**Alive:**
- Target's profile image
- Target's real name + username
- "Confirm kill" / "I was killed" buttons → push [Confirm Kill](confirmKill.md)

**Eliminated:**
- Killer's name + image + when they killed you
- No action buttons

### 3. Own kills list
- One row per kill the logged-in player has made in this game
- Each row: victim name + kill time
- Empty-state placeholder when no kills yet

## Notes
- Replaces the original `Target Info` + `Kills Info` pages — both fit on this single dashboard.
- Living/dead-players counters and the dead-players list from the original Gotcha are dropped — those belong on the Match Result screen, not here. Player Home stays focused on "what does *this* player need to know right now".
- No visibility toggles (`ShowPlayerImages`, `ShowGender`, `ShowHunter`), no weapon, no custom rules — the screen always shows the basics.
