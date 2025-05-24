namespace WikaApp.Auth;

public static class HttpContextExt
{
  public static bool is_authenticated(this HttpContext context) =>
    context.User.Identity?.IsAuthenticated ?? false;

  public static string get_email(this HttpContext context) =>
    context.User
      .FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
      ?.Value ?? string.Empty;

  public static string get_name(this HttpContext context) =>
    context.User.FindFirst("name")?.Value ?? string.Empty;

  public static string get_picture(this HttpContext context) =>
    context.User.FindFirst("picture")?.Value ?? string.Empty;
}