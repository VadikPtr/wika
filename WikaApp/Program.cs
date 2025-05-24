using Microsoft.EntityFrameworkCore;
using WikaApp;
using WikaApp.Auth;
using WikaApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services
  .AddDbContextFactory<AppDbContext>()
  .AddDbContext<AppDbContext>()
  .AddSingleton<AppConfig>()
  .AddSingleton<IConfiguration>(_ => create_configuration())
  .configure_authentication_and_authorization(create_configuration());

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  app.UseHsts();
}
app.UseAntiforgery();
app.use_authentication_and_authorization();
app.UseStaticFiles(); // NOTE: Serve files from wwwroot
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

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