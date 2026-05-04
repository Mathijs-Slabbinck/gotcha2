# Index (Sign In)

The index page is the landing page for unauthenticated users.

**The page has 2 sections:**
- Sign In form
- Welcome message

**Sign In form contains:**
- Username or email input field
- Password input field
- Sign In button

**Welcome section contains:**
- Welcome message ("Please log in to continue.")
- Link to sign up page ("If you don't have an account yet, you can sign up.")
- Sign Up button

**Notes:**
- No "forgot password" flow in Gotcha2 (intentionally stripped — keeps the auth scope minimal).
- Lockout: after 5 failed sign-in attempts the account is locked for 15 minutes (handled API-side).
