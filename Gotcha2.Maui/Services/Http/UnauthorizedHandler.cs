using System.Net;

namespace Gotcha2.Maui.Services.Http
{
    // Inbound DelegatingHandler — reacts to 401s by signing the user out. Sits INNER in the chain (closer to the wire).
    public class UnauthorizedHandler : DelegatingHandler
    {
        private readonly SessionService _sessionService;

        public UnauthorizedHandler(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // protected: required because the base declares it protected.
        // override: required because we're changing the behavior of the base class's SendAsync
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /* 401 without a Bearer header = sign-in failure;
             * the ViewModel owns that message and we must NOT yank the (already empty) session.
             * 401 WITH a Bearer header means the server rejected our token - sign the user out. */
            bool requestHadToken = request.Headers.Authorization?.Scheme == AuthHeaderHandler.BearerScheme;

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && requestHadToken)
                await MainThread.InvokeOnMainThreadAsync(async () => await _sessionService.SignOutAsync());

            return response;
        }
    }
}
