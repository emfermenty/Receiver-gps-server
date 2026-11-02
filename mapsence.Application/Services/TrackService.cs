using mapsence.Domain.Models;
using mapsence.Application.DTO;
using mapsenceto.Application.Services;

namespace mapsence.Application.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly ITrackPointInfoRepository _trackPointInfoRepository;
    private readonly ITrackViewRepository _trackViewRepository;
    //private readonly ISensorRepository _sensorRepository;

    public TrackService(
        ITrackRepository trackRepository,
        ITrackPointInfoRepository trackPointInfoRepository,
        ITrackViewRepository trackViewRepository)
        //ISensorRepository sensorRepository)
    {
        _trackRepository = trackRepository;
        _trackPointInfoRepository = trackPointInfoRepository;
        _trackViewRepository = trackViewRepository;
        //_sensorRepository = sensorRepository;
    }
    public async Task<Track?> FindTrackByIdAsync(int id)
    {
        return await _trackRepository.FindTrackByIdAsync(id);
    }

    public async Task<List<TrackDisplay>> FindByUserId(int userId)
    {
        var views = await _trackViewRepository.FindByUserId(userId);
        var displays = new List<TrackDisplay>();
        foreach (var view in views)
        {
            displays.Add(TrackDisplay.Create
            (
                view.Id,
                view.Name,
                view.DateStart.AddHours(5),
                view.DateStop.HasValue ? view.DateStop.Value.AddHours(5) : null,    
                view.SensorId, 
                view.UserId, 
                view.Distance,
                view.AvgSpeed, 
                view.Duration
                ));
        }
        return displays;
    }

    public async Task<List<TrackPointInfo>> FindPointsByTrack(int trackId)
    {
        return await _trackPointInfoRepository.FindByTrackIdOrderByDateTime(trackId);
    }

    public async Task DeleteTrack(int trackId)
    {
        await _trackRepository.PrcDeleteTrackAsync(trackId);
    }

    public async Task EditTrack(int trackId, string name)
    {
        await _trackRepository.PrcEditTrackAsync(trackId, name);
    }

    // Точки привязываются к треку по интервалу date_start..date_stop (см. v_track_info),
    // поэтому ReceiverApp не нужно сообщать о начале и конце трека
    public async Task StartTrack(StartTrackRequest request)
    {
        await _trackRepository.PrcStartTrackAsync(request);
    }

    public async Task StopTrack(int trackId)
    {
        var dateStop = DateTime.UtcNow;
        await _trackRepository.PrcStopTrackAsync(trackId, dateStop);
    }

    public async Task ComebackTrack(int trackId)
    {
        await _trackRepository.PrcComebackTrackAsync(trackId);
    }
}
