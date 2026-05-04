# Match Result

The Match Result page is for the Player (logged in, in a finished game). Reached by tapping a finished game on the Games list (`?gameId=...`).

## Sections (top to bottom)

### 1. Winner — big, prominent
- Winner's profile image (large)
- Winner's name + username
- Game name + start/end dates

### 2. Players list
- One row per player in the game, sorted by kill count
- Each row shows:
  - Profile image
  - Name (username below)
  - Status — `Killed by <name>` (regular player) or `Winner` (last surviving player)
- **Tapping a player row** updates the kill table below to show that player's kills.
- The winner's row is selected by default on page load.

### 3. Kill table (filtered by selected player)
- Header: "<Selected player>'s kills"
- One row per kill the selected player made: victim name + kill time
- Empty-state placeholder if the selected player has no kills

## Mobile features used
- **Share** — a "Share results" button at the top opens the native share sheet with the final standings as plain text.

## Notes
- Replaces the separate `Kills Info` page — kill details now live inline, filtered by the tapped player.
- Stats (number of players, total kills, dates) are folded into the winner header to keep the page compact on phone portrait.
