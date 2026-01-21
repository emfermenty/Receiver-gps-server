using mapsence.Domain.Models;
using mapsenceto.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace mapsenceToAsp.Api.Controllers;

[ApiController]
[Route("/sensor")]
public class SensorController : ControllerBase
{
    private readonly ISensorService _sensorService;

    public SensorController(ISensorService sensorService)
    {
        _sensorService = sensorService;
    }

    [HttpGet("/all")]
    public async Task<ActionResult<List<Sensor>>> FindAllSensors()
    {
        return Ok(await _sensorService.FindAllSensorsAsync());
    }
    [HttpGet("/current/{sensorId}")]
    public async Task<ActionResult<GeoData>> CurrentGeoData(int sensorId)
    {
        return Ok(await _sensorService.CurrentGeoDataOfSensorsAsync(sensorId));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<List<Sensor>>> FindByUserId(int userId)
    {
        return Ok(await _sensorService.FindByUserIdAsync(userId));
    }
}