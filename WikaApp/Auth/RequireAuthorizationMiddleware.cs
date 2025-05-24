using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace WikaApp.Auth;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequireAuthorizationAttribute : Attribute;

public sealed class RequireAuthorizationMiddleware(
  RequestDelegate next,
  IServiceProvider sp,
  ILogger<RequireAuthorizationMiddleware> logger)
{
  // ReSharper disable once InconsistentNaming
  public async Task InvokeAsync(HttpContext context)
  {
    var endpoint  = context.Features.Get<IEndpointFeature>()?.Endpoint;
    var attribute = endpoint?.Metadata.GetMetadata<RequireAuthorizationAttribute>();

    if (attribute != null)
    {
      if (!context.is_authenticated())
      {
        context.Response.Redirect("/auth/login");
        return;
      }

      var       email      = context.get_email();
      using var scope      = sp.CreateScope();
      var       db_context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      if (!await db_context.users.AnyAsync(x => x.email == email))
      {
        logger.LogDebug("No email address found in database: {}.", email);
        context.Response.Redirect("/auth/sorry");
        return;
      }
    }

    await next(context);
  }
}