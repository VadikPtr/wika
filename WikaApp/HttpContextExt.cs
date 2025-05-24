namespace WikaApp;

public static class HttpContextExt
{
  public static bool is_authenticated(this HttpContext context)
  {
    // var user = context.User.FindFirst("name")?.Value;
    // if (string.IsNullOrEmpty(user))
    return context.User.Identity?.IsAuthenticated ?? false;
  }
}