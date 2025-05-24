using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;

namespace WikaApp;

public class AppDbAuthorizationMiddleware(
  AppDbContext dbContext,
  ILogger<AppDbAuthorizationMiddleware> logger
) : IAuthorizationMiddlewareResultHandler {
  private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

  public async Task HandleAsync(
    RequestDelegate next,
    HttpContext context,
    AuthorizationPolicy policy,
    PolicyAuthorizationResult authorizeResult)
  {
    var email = context.User
      .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
      ?.Value;
    if (email != null && !await dbContext.users.AnyAsync(x => x.email == email))
    {
      logger.LogDebug("No email address found in database: {}.", email);
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      return;
    }

    await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
  }
}