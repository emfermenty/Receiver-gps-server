namespace Receiver_server;

/// <summary>
/// TCP-подключение трекера: с какого адреса и когда подключился.
/// </summary>
public record SessionInfo(string Ip, DateTime StartedAt);
