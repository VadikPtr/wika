using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WikaApp;
using WikaApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services
  .AddDbContextFactory<AppDbContext>()
  .AddDbContext<AppDbContext>()
  .AddSingleton<AppConfig>()
  .AddSingleton<IConfiguration>(_ => create_configuration());

builder.Services
  .AddAuthentication(o => {
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme    = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme          = CookieAuthenticationDefaults.AuthenticationScheme;
  })
  .AddCookie()
  .AddGoogleOpenIdConnect(options =>
  {
    var app_config = new AppConfig(create_configuration());
    options.ClientId             = app_config.oauth_client_id;
    options.ClientSecret         = app_config.oauth_client_secret;
    options.CallbackPath         = "/signin-oidc";
    options.SignedOutRedirectUri = "/logout";
  });

builder.Services.Configure<CookieAuthenticationOptions>(
CookieAuthenticationDefaults.AuthenticationScheme,
options => {
  options.Cookie.SameSite     = SameSiteMode.None;
  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
}
);
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddTransient<IAuthorizationMiddlewareResultHandler, AppDbAuthorizationMiddleware>();

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  app.UseHsts();
}

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // NOTE: Serve files from wwwroot

app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  var logger  = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
  scope.ServiceProvider.GetRequiredService<AppConfig>(); // NOTE: warmup

  context.Database.EnsureCreated();

  if (context.Database.GetPendingMigrations().Any())
    context.Database.Migrate();

  logger.LogDebug("Database model: {}", context.Model.ToDebugString());
}

app.Run();
return;

IConfiguration create_configuration()
{
  var b = new ConfigurationBuilder();
  b.AddJsonFile("appsettings.json");
  if (builder.Environment.IsDevelopment())
    b.AddJsonFile("appsettings.Development.json");
  b.AddEnvironmentVariables();
  return b.Build();
}