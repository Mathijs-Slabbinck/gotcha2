# Settings

The settings page is for the User (when logged in but not in a game).

**Editable fields:**
- First name
- Last name
- Username
- Email
- Profile image (Take photo / Choose from gallery → camera/gallery integration)
- Password (current + new + confirm)

**Notes:**
- Birthday is **not** editable (the original Gotcha allowed it once; Gotcha2 makes it locked-after-signup for simplicity).
- Account deletion is **not** in scope for Gotcha2 (was in the original Gotcha as soft-delete + anonymise; deferred to keep auth lean).
- All updates go through the API; the MAUI `ApiAuthService` (or a new `IUserService`) carries them.
