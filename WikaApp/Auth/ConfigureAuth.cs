using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WikaApp.Auth;

public static class ConfigureAuth
{
  public static IServiceCollection configure_authentication_and_authorization(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    services
      .AddAuthentication(o =>
      {
        o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        o.DefaultForbidScheme    = GoogleOpenIdConnectDefaults.AuthenticationScheme;
        o.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie(o =>
      {
        o.Cookie.SameSite     = SameSiteMode.None;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.SlidingExpiration   = true;
      })
      .AddGoogleOpenIdConnect(o =>
      {
        var app_config = new AppConfig(configuration);
        o.ClientId                = app_config.oauth_client_id;
        o.ClientSecret            = app_config.oauth_client_secret;
        o.CallbackPath            = "/signin-oidc";
        o.Events.OnTicketReceived = context =>
        {
          context.Properties!.IsPersistent = true;
          return Task.CompletedTask;
        };
      });

    services
      .AddAuthorization()
      .AddCascadingAuthenticationState();

    return services;
  }

  public static void use_authentication_and_authorization(this WebApplication app)
  {
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<RequireAuthorizationMiddleware>();
  }
}