namespace Restaurant.Messaging.Configuration;

public sealed class RabbitSettings
{
    public string Host { get; set; } = string.Empty;

    public ushort Port { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

}
