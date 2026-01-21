using mapsence.Application.DTO;
using mapsence.Application.Services;
using mapsence.Domain.Models;
using mapsenceto.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace mapsenceToAsp.Api.Controllers;

[ApiController]
[Route("/track")]
public class TrackController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TrackController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    [HttpGet("user_track/{user_id}")]
    public async Task<ActionResult<List<TrackDisplay>>> FindByUserId(int user_id)
    {
        return Ok(await _trackService.FindByUserId(user_id));
    }

    [HttpGet("coordinates/{trackId}")]
    public async Task<ActionResult<List<TrackPointInfo>>> GetTrackCoordinates(int trackId)
    {
        return Ok(await _trackService.FindPointsByTrack(trackId));
    }

    [HttpDelete("delete/{trackId}")]
    public async Task<ActionResult> DeleteTrack(int trackId)
    {
        await _trackService.DeleteTrack(trackId);
        return NoContent();
    }

    [HttpPut("edit/{trackId}")]
    public async Task<ActionResult> EditTrack(int trackId, [FromBody] EditTrackRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _trackService.EditTrack(trackId, request.NewName);
        return NoContent();
    }

    [HttpPost("stop/{trackId}")]
    public async Task<ActionResult> StopTrack(int trackId)
    {
        await _trackService.StopTrack(trackId);
        return NoContent();
    }

    [HttpPost("comeback/{trackId}")]
    public async Task<ActionResult> ComebackTrack(int trackId)
    {
        await _trackService.ComebackTrack(trackId);
        return NoContent();
    }

    [HttpPost("start/{sensorId}")]
    public async Task<ActionResult> StartTrack([FromBody] StartTrackRequest request)
    {
        await _trackService.StartTrack(request);
        return NoContent();
    }
}