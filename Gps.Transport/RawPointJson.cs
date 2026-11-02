using System.Text.Json;

namespace Gps.Transport;

/// <summary>
/// Формат строки при передаче, одинаковый для Kafka и сокета.
/// </summary>
public static class RawPointJson
{
    public static byte[] Serialize(RawPointDto point) => JsonSerializer.SerializeToUtf8Bytes(point);

    public static RawPointDto Deserialize(byte[] payload) =>
        JsonSerializer.Deserialize<RawPointDto>(payload) ?? throw new JsonException("Пустая строка raw_points");
}
