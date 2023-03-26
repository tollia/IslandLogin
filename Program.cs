using IslandLogin.Classes.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;

IConfiguration configuration = new ConfigurationBuilder()
    //.AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.secret.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(options => {
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddOpenIdConnect(options => {
        options.ResponseType = "code";
        options.ResponseMode = "form_post";
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.SaveTokens = true;
        options.CallbackPath = "/signin-oidc";
        options.UsePkce = true;
        configuration.GetSection("Oidc").Bind(options);
        options.Events = new OpenIdConnectEvents {
            OnTokenValidated = context => {
                if (context == null) {
                    throw new InvalidOperationException("The context should never be null in the OnTokenValidated event.");
                } else {
                    string token = context.SecurityToken.RawData;
                    string tokenExpire = context.SecurityToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value ?? "0";
                    if (context.Principal == null) {
                        throw new InvalidOperationException("The context.Principal should never be null in the OnTokenValidated event.");
                    }

                    // This needs review but simplest way yet found to maintain access to the original JWT ID Token
                    // through the lifetime of this authentication. The exires_at Claim can probably be removed.
                    context.Principal.AddIdentity(
                        new ClaimsIdentity(
                            new[] {
                                new Claim("id_token", token),
                                new Claim("exires_at", tokenExpire)
                            }
                        )
                    );
                }

                // Put any last second failures based on previous events in the pipline here.
                // This is the last place you get to fail the authentication.
                //if (someErrorCondition) {
                //    context.Fail(new Exception("Your error message"));
                //}

                return Task.CompletedTask;
            },
            OnRedirectToIdentityProvider = context =>
            {
                // Useful for debugging breakpoints and to tweak things before the Identity Privider is called.
                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(options => {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(4); // Set the cookie expiration time
        options.SlidingExpiration = true; // Enable sliding expiration
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<AccessTokenExpirationMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
