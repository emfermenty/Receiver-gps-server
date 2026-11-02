namespace Receiver_server;

/// <summary>
/// Описание трекера из handshake-пакета.
/// </summary>
public record DeviceInfo(string Imei, string Manufacturer, string Model, string Firmware);
