namespace Websocket.Configuration;

public static class WebSocketConfiguration
{
    public const string Name = "Chat";
    public static string Port { get; } = "5000";
    public static byte[] Buffer { get; } = new byte[1024 * 4];
    public static int KeepAliveIntervalSeconds { get; set; } = 120;
}