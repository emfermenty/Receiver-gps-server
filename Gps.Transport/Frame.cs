using System.Buffers.Binary;

namespace Gps.Transport;

/// <summary>
/// Протокол сокета между экспортом и импортом: строка передаётся как [длина:4][данные],
/// получатель отвечает одним байтом подтверждения после того, как записал её в БД.
/// </summary>
public static class Frame
{
    private const int MaxLength = 1024 * 1024;
    private const byte Ack = 1;

    public static async Task WriteAsync(Stream stream, byte[] payload, CancellationToken token)
    {
        var header = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(header, payload.Length);
        await stream.WriteAsync(header, token);
        await stream.WriteAsync(payload, token);
    }

    public static async Task<byte[]> ReadAsync(Stream stream, CancellationToken token)
    {
        var header = new byte[4];
        await stream.ReadExactlyAsync(header, token);

        int length = BinaryPrimitives.ReadInt32LittleEndian(header);
        if (length is <= 0 or > MaxLength)
            throw new InvalidDataException($"Некорректная длина сообщения: {length}");

        var payload = new byte[length];
        await stream.ReadExactlyAsync(payload, token);
        return payload;
    }

    public static async Task WriteAckAsync(Stream stream, CancellationToken token)
    {
        await stream.WriteAsync(new[] { Ack }, token);
    }

    public static async Task ReadAckAsync(Stream stream, CancellationToken token)
    {
        var ack = new byte[1];
        await stream.ReadExactlyAsync(ack, token);
        if (ack[0] != Ack)
            throw new InvalidDataException($"Некорректное подтверждение: {ack[0]}");
    }
}
