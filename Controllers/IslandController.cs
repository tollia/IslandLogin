using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IslandLogin.Controllers {
    [Route("signin-oidc")]
    public class IslandController : Controller {

        public async Task<IActionResult> Callback(string returnUrl = "/") {
            var authResult = await HttpContext.AuthenticateAsync();

            if (!authResult.Succeeded) {
                // Handle authentication error.
                return BadRequest();
            }

            await HttpContext.SignInAsync(authResult.Principal, authResult.Properties);

            return Redirect(returnUrl);
        }
    }
}
