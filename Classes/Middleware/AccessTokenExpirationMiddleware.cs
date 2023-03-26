using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace IslandLogin.Classes.Middleware {
    // Needs review, this was an early attempt at having the cookie contained validity match that of the issued 
    // JWT validity. This is very short, 5 minutes, and will lead to horible user experiences. Time should rather
    // be spent on looking into the OIDC renewal mechanism and see if it provides us with some sort of solution for this.
    public class AccessTokenExpirationMiddleware {
        private readonly RequestDelegate _next;

        public AccessTokenExpirationMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            //bool needsLogin = false;

            //var claims = context.User.Claims;
            //string accessToken = claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            //if (accessToken == null) {
            //    needsLogin = true;
            //} else {
            //    var token = new JwtSecurityToken(jwtEncodedString: accessToken);
            //    needsLogin = true;
            //}

            //if (user.Identity.IsAuthenticated) {
            //    var expiresAt = user.Claims.FirstOrDefault(c => c.Type == "expires_at")?.Value;
            //    if (!string.IsNullOrEmpty(expiresAt)) {
            //        var tokenExpirationTime = DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);
            //        if (tokenExpirationTime <= DateTimeOffset.UtcNow) {
            //            // Access token has expired, trigger re-authentication
            //            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
            //            return;
            //        }
            //    }
            //}

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
