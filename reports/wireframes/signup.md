# Sign Up

The sign up page is for unauthenticated users to create a new account.

**The page has a single form with the following fields:**
- First name
- Last name
- Username
- Email address
- Gender (dropdown)
- Birthday (date picker)
- Password
- Confirm password
- Profile image (optional — "Take photo" or "Choose from gallery" buttons → camera/gallery integration)

**Below the form:**
- Sign Up button
- "Already have an account?" link (leads to Sign In page)

**Validation (FluentValidation, client-side + server-side):**
- All non-image fields are required.
- Email must be a valid email.
- Password rules match the API's Identity policy (minimum length, mix of casing/digits/symbols).
- Birthday: not in the future, age ≥ 13 (digital-consent age).
- Profile image is optional — if provided, capture happens via the **Camera** integration (`MediaPicker.CapturePhotoAsync()` / `PickPhotoAsync()`).

**Mobile features used:** Camera (platform integration #1).
