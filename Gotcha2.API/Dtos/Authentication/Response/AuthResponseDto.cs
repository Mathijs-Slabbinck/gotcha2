namespace Gotcha2.API.Dtos.Authentication.Response
{
    // Used in: AuthController
    // For: Register, Login, GenerateTokenAsync
    // What the API sends back when a user successfully authenticates (login or register).
    // The client stores the token (e.g. in localStorage / a Postman variable) and
    // sends it on every subsequent request as `Authorization: Bearer <token>`.
    public class AuthResponseDto
    {
        // The encoded JWT string itself (header.payload.signature).
        public required string Token { get; init; }

        // When the token expires (UTC). Useful for the client to know when to re-login.
        public required DateTime ExpiresAtUtc { get; init; }
    }
}
