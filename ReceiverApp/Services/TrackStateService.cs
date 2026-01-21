using System.Collections.Concurrent;

public class TrackStateService
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, int>> _activeTracks = new();

    public void StartRecording(string imei, int trackId, int userId)
    {
        var tracks = _activeTracks.GetOrAdd(imei, _ => new ConcurrentDictionary<int, int>());
        tracks[trackId] = userId;
        Console.WriteLine($"Запись для {imei} начата. TrackId={trackId}, UserId={userId}");
    }

    public void StopRecording(string imei, int trackId)
    {
        if (_activeTracks.TryGetValue(imei, out var tracks))
        {
            if (tracks.TryRemove(trackId, out _))
            {
                Console.WriteLine($"Запись для {imei} остановлена. TrackId={trackId}");

                // удаляем IMEI только если треков больше нет
                if (tracks.IsEmpty)
                {
                    _activeTracks.TryRemove(imei, out _);
                }
            }
            else
            {
                Console.WriteLine($"Трек {trackId} для {imei} не найден.");
            }
        }
        else
        {
            Console.WriteLine($"IMEI {imei} не найден в активных треках.");
        }
    }

    public bool HasAnyActiveTrack(string imei) =>
        _activeTracks.TryGetValue(imei, out var tracks) && !tracks.IsEmpty;

    public IEnumerable<(int trackId, int userId)> GetActiveTracks(string imei)
    {
        if (_activeTracks.TryGetValue(imei, out var tracks))
        {
            return tracks.Select(t => (t.Key, t.Value));
        }
        return Enumerable.Empty<(int, int)>();
    }
}