using mapsence.Domain.Models;
using mapsenceto.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mapsenceToAsp.Api.Controllers;

[ApiController]
[Route("api/geodata")]
[Authorize]
public class GeoDataController : ControllerBase
{
    private readonly IGeoDataService _geoDataService;
    
    public GeoDataController(IGeoDataService geoDataService)
    {
        _geoDataService = geoDataService;
    }
    
    [HttpGet("sensor/{id}")]
    [Authorize(Policy = "EveryOne")]
    public async Task<ActionResult<List<GeoData>>> FindGeoDataBySensor(int id)
    {
        var result = await _geoDataService.FindBySensorId(id);
        return Ok(result);
    }
}