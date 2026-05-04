# Navigation / Areas

Gotcha2 uses MAUI Shell with **two TabBars** (one for unauthenticated users, one for authenticated users). The active TabBar is swapped based on `SessionService.IsSignedIn`.

## Unauthenticated
Pages shown before the user is logged in.

**On TabBar:**
- Sign In (Index)
- Sign Up
- Info

## Authenticated

### User (logged in, not in a specific game)
**On TabBar:**
- Home (profile)
- Games (list)
- Settings

**Not on TabBar (pushed routes):**
- New Game (via the "Create new game" button on Games)

### Player (logged in, in a specific game)
Reached by tapping a game on the Games list. The selected `gameId` is passed as a query-string parameter on the pushed route (e.g. `PlayerHome?gameId=...`); the receiving page implements `IQueryAttributable` (or uses `[QueryProperty]`) to read it. **No game-id is held in `SessionService`** — every player-scoped page is responsible for its own gameId via the route.

**Ongoing games:**
- Player Home (default screen — player header + current target + own kills list)
- Confirm Kill (pushed from Player Home)

**Finished games:**
- Match Result (default screen — winner + players list + kill table that updates when tapping a player)

## What's NOT here (compared to original Gotcha)

- ❌ Reset Password — auth scope intentionally trimmed.
- ❌ Admin (per-game admin tools) — Gotcha2 has no per-game admin role.
- ❌ Shop / Store — no in-app purchases.
- ❌ User-side dashboard with VIP unlock store.
- ❌ Contact page — cut from Gotcha2 scope.
- ❌ Separate Kills Info page — folded into Match Result (tap a player to filter the kill table).
- ❌ Separate Target Info page — folded into Player Home (target + own kills on one screen).

## Notes

- Route names are constants in `Gotcha2.Maui/Routes.cs`. Never inline route strings.
- Absolute routes (TabBar) are prefixed with `//`. Relative routes (pushed onto current tab) have no prefix.
- Routes that aren't TabBar-mounted must be `Routing.RegisterRoute(Routes.X, typeof(XPage))`'d in `MauiProgram.CreateMauiApp()` before they're navigable.
