# New Game

The New Game page is for the User (when logged in but not in a game). Reached via the "Create new game" button on the Games page.

**The page has a simple form:**
- Game name (text input — required, default "New game")
- Create Game button

**After creating:**
- The user is sent back to the Games page.
- The new game appears in the **Pending games** section.
- The creator can now tap **Share invite link** to invite players.

**Notes:**
- The original Gotcha had ~20 rule toggles (chaos mode, assassin, custom kill methods, timed kills, …). Gotcha2 strips all of this — there is **no `Rules` entity**, no game modes. Only the core kill flow.
- The game creator is automatically added as the first `Player` (with `IsCreator` derived from `Game.CreatorId`).
