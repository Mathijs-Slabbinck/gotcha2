# Home (User Profile)

The home page is for the User (when logged in but not in a game). It's the landing screen after login.

**The page shows the user's profile with:**
- Profile image (large, circular, centered) — placeholder if not set.
- Welcome message with username ("Welcome, TheLegend27").

**Stats section with the following info:**
- Games played
- Games won
- Total kills
- Account created date

**Notes:**
- Stats are computed from the user's `Player` rows + linked `Kill` rows (read-only, no separate stats entity).
- "Biggest kill streak" from the original Gotcha is dropped — too complex to compute without dedicated state.
