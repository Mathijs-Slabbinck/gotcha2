# Games

The games page is for the User (when logged in but not in a specific game).

**The Games page has 4 sections:**
- **Pending games** — games the user has joined or created that haven't started yet.
- **Active games** — games that are currently running (the user is alive or dead, both shown).
- **Finished games** — games that have ended.
- **Create new game** — button at the top or bottom that navigates to the New Game page.

**Each game row shows:**
- Game name
- Number of players
- Status (pending / active / finished)
- For active games: whether the user is alive
- For finished games: who won

**Tapping a row:**
- For active/pending: pushes [Player Home](playerHome.md) with `?gameId=...`.
- For finished: pushes [Match Result](matchResult.md) with `?gameId=...`.

**Mobile features used:**
- **Share** — for pending games the **creator** sees a "Share invite link" button next to the game name. Tapping it opens the native share sheet with the join URL.
