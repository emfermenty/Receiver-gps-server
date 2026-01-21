using mapsence.Domain.Models;
using mapsence.Application.DTO;
using mapsenceto.Application.Services;

namespace mapsence.Application.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly ITrackPointInfoRepository _trackPointInfoRepository;
    private readonly ITrackViewRepository _trackViewRepository;
    private readonly IKafkaProducerService _producer;
    //private readonly ISensorRepository _sensorRepository;

    public TrackService(
        ITrackRepository trackRepository,
        ITrackPointInfoRepository trackPointInfoRepository,
        ITrackViewRepository trackViewRepository,
        IKafkaProducerService producer)
        //ISensorRepository sensorRepository) 
    {
        _trackRepository = trackRepository;
        _trackPointInfoRepository = trackPointInfoRepository;
        _trackViewRepository = trackViewRepository;
        _producer = producer;
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

    public async Task StartTrack(StartTrackRequest request)
    {
        var id = await _trackRepository.PrcStartTrackAsync(request);
        await _producer.SendMessageAsync($"START:{id}:{request.UserId}:862549040911052");
    }

    public async Task StopTrack(int trackId)
    {
        var dateStop = DateTime.UtcNow;
        await _trackRepository.PrcStopTrackAsync(trackId, dateStop);
        await _producer.SendMessageAsync($"STOP:862549040911052:{trackId}");
    }

    public async Task ComebackTrack(int trackId)
    {
        await _trackRepository.PrcComebackTrackAsync(trackId);
    }
}
