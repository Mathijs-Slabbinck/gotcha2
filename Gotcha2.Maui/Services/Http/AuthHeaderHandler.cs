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
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}
