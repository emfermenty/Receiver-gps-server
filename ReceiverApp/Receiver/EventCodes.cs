namespace Receiver_server.Receiver;

/// <summary>
/// Коды событий из байта 3 геопакета.
/// </summary>
public static class EventCodes
{
    private static readonly Dictionary<int, string> Names = new()
    {
        [0] = "Периодическая",
        [1] = "Поворот",
        [2] = "Остановка",
        [3] = "Начало движения",
        [4] = "Низкий заряд",
        [5] = "SOS",
    };

    public static string GetName(int code) =>
        Names.TryGetValue(code, out var name) ? name : $"Неизвестное событие {code}";
}
