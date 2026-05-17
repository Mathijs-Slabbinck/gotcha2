using System.Net.Http.Headers;

namespace Gotcha2.Maui.Services.Http
{
    // Outbound DelegatingHandler - stamps the JWT on every request that has one. Sits OUTER in the chain.
    public class AuthHeaderHandler : DelegatingHandler
    {
        // Const because UnauthorizedHandler reads it back to detect "did we send a token on this request?".
        public const string BearerScheme = "Bearer";

        private readonly SessionService _sessionService;

        public AuthHeaderHandler(SessionService sessionService)
        {
            _sessionService = sessionService;
        }


        // protected: required because the base declares it protected.
        // override: required because we're changing the behavior of the base class's SendAsync
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = _sessionService.CurrentAuthToken;

            // No token = anonymous endpoint (sign-in / sign-up).
            // Leaving the header off lets UnauthorizedHandler distinguish that from a real 401 on a token-bearing call.
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);

            // Bypass the dev tunnel's HTML interstitial page — without this, the first request from any client
            // gets a "do you want to access this tunnel?" page back instead of JSON, and ReadFromJsonAsync throws.
            // No-op when BaseUrl points to a non-tunnel host.
            request.Headers.Add("X-Tunnel-Skip-Anti-Phishing-Page", "true");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
