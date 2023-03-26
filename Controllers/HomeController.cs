using IslandLogin.Classes.Network;
using IslandLogin.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace IslandLogin.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            var claimsTypeValue = User.Claims.Select(c => new { Type = c.Type, Value = c.Value }).ToList();
            ViewData["ClaimsJson"] = JsonConvert.SerializeObject(claimsTypeValue, Formatting.Indented);
            return View();
        }

        public IActionResult WeatherForecast() {
            string idToken = User.Claims.ToList().FirstOrDefault(x => x.Type.Equals("id_token", StringComparison.Ordinal))?.Value;
            var httpClientHandler = new AuthenticatedHttpClientHandler(idToken);
            HttpClient httpClient = new HttpClient(httpClientHandler);

            OidcDownstreamTokenVerifier.Service service = new(
                "https://localhost:8001",
                httpClient
            );

            ICollection<OidcDownstreamTokenVerifier.WeatherForecast> forecastCollection = service.GetWeatherForecastAsync().Result;

            ViewData["WeatherForecastJson"] = JsonConvert.SerializeObject(forecastCollection, Formatting.Indented);
            return View();
        }

        public IActionResult SignOut() {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}