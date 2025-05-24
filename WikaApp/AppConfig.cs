namespace WikaApp;

public class AppConfig(IConfiguration configuration)
{
  public string oauth_client_id     { get; } = get_value(configuration, "oauth_client_id");
  public string oauth_client_secret { get; } = get_value(configuration, "oauth_client_secret");

  private static string get_value(IConfiguration configuration, string key)
  {
    Console.WriteLine($"CWD: {Environment.CurrentDirectory}");
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
      throw new Exception($"Missing configuration value for key: {key}");
    return value;
  }
}