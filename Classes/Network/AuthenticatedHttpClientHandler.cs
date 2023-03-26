using IdentityModel.Client;

namespace IslandLogin.Classes.Network {
    public class AuthenticatedHttpClientHandler : HttpClientHandler {
        private readonly string _accessToken;

        public AuthenticatedHttpClientHandler(string accessToken) {
            _accessToken = accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            request.SetBearerToken(_accessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
