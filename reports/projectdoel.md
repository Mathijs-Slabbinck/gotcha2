# Project Goals — MDE

## Description

### What is Gotcha?
Gotcha (also known as Assassin) is a social elimination game played in big groups. Every player gets assigned a secret target. Your goal? "Eliminate" your target by tagging them or catching them with a chosen weapon. Once you succeed, your target is out and you take on *their* target. The game continues until only one player remains.

### What the app offers
Gotcha is great for big groups but setting it up is a hassle — printing names, assigning targets on paper, keeping track of who killed whom. **This app removes all that hassle.** Players join a game, the app assigns targets automatically, players confirm kills, and the app keeps the standings up to date in real time.

This is the **stripped-down version** of an earlier Gotcha attempt that grew too big. Scope is intentionally cut to the **core kill-game flow only** — no game-rule variants, no admin panel, no store, no in-app purchases, no logging entities. Just: sign up, create or join a game, see your target, confirm kills, and see who wins.

### FAQ
- **Goal of the app:** make running a Gotcha game effortless for the organiser and intuitive for the players.
- **Problem solved:** setting up Gotcha on paper is annoying, error-prone, and limited.
- **Who uses it:** big groups that play Gotcha — youth movements, schools, companies on team-building days.
- **How it's used:** mainly to track game state and confirm kills as they happen, on each player's phone.

## Online strategie

Kruis je online **strategie** aan:

- [ ] Online CRUD operaties met een Backend Service
- [ ] Online Fetch, Offline CRUD
- [ ] Offline CRUD, Online Push
- [x] Online CRUD operaties met eigen REST API
- [ ] Andere, namelijk:

(***Verduidelijking keuze:** Gotcha2 wordt real-time gespeeld, dus de app moet altijd online zijn. We schrijven onze eigen REST API zodat we volledige controle hebben over het datamodel en de auth-laag.*)

## Mobile features

Kruis je geplande **mobile features** aan:

- [x] Platformintegraties
      noteer welke: **Camera** (profielfoto's), **Share** (uitnodigingslink delen + eindstand delen), **Connectivity** (offline-banner in `AppShell`)
- [ ] Push notifications
- [ ] 2D Graphics
- [x] Authentication en Authorization
- [ ] Native Communication
- [ ] Native Speech to Text
- [ ] Cross-platform Native Plugin
- [ ] Andere, namelijk:

### Verduidelijking keuze mobile features

**Authentication en Authorization** — JWT-gebaseerd via een eigen `AuthController` op de API. Login, register, lockout na 5 mislukte pogingen. Permissies enforced via `User.GetUserId()` op de API zodat een gebruiker alleen z'n eigen games en kills kan zien/wijzigen.

**Platformintegraties — drie integraties** (≥ 2 vereist):

- **Camera** — bij signup en in Settings kan de gebruiker een profielfoto nemen via de native camera (`MediaPicker.CapturePhotoAsync()`) of er één kiezen uit de gallery (`MediaPicker.PickPhotoAsync()`).
- **Share** — op het Games-scherm en op MatchResult kan de game-creator de **invite link** of de **eindstand** delen via de native share sheet (`Share.Default.RequestAsync(...)`). Dit is hoe nieuwe spelers een lopende game joinen.
- **Connectivity** — in `AppShell` toont de app een offline-banner zodra `Connectivity.Current.NetworkAccess` niet `Internet` is. Dit voorkomt dat een speler een kill probeert te bevestigen zonder verbinding.

## Cross-platform

|   Platform   |       Status       |                                  Test target                                  |
|:------------:|:------------------:|:-----------------------------------------------------------------------------:|
|  **Android** |   primair target   |  Emulator (Android Studio AVD) tijdens dev, fysieke telefoon voor demo        |
|  **Windows** |   primair target   |  Lokaal op laptop                                                             |
|     iOS      |    geen target     |  Geen Apple-hardware beschikbaar                                              |

## Wireframes

### Unauthenticated
- [Index (Sign In)](wireframes/index.md)
- [Sign Up](wireframes/signup.md)
- [Info](wireframes/info.md)

### User (authenticated, not in game)
- [Home (profile)](wireframes/home.md)
- [Settings](wireframes/settings.md)
- [Games (list)](wireframes/games.md)
- [New Game](wireframes/newGame.md)

### Player (authenticated, in game — pushed with `gameId` query parameter)
- [Player Home](wireframes/playerHome.md)
- [Confirm Kill](wireframes/confirmKill.md)
- [Match Result](wireframes/matchResult.md)
