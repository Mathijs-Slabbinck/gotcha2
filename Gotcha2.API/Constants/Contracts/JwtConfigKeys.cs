namespace Gotcha2.API.Constants.Contracts
{
    public static class JwtConfigKeys
    {
        // Config-section paths for JWT settings in appsettings.json.
        public const string Key = "Jwt:Key";
        public const string Issuer = "Jwt:Issuer";
        public const string Audience = "Jwt:Audience";
        public const string ExpiresInMinutes = "Jwt:ExpiresInMinutes";
    }
}
