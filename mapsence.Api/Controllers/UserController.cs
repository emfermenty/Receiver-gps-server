using mapsence.Application.DTO;
using mapsence.Application.Services;
using mapsence.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace mapsenceToAsp.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginModelDTO request)
    {
        var reg = userService.Register(request.Username, request.Password);
        if (await reg == true)
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO model)
    {
        var token = await userService.Login(model.Username, model.Password);
        HttpContext.Response.Cookies.Append("token", token,new CookieOptions
        {
            HttpOnly = true,
        });
        return Ok(token);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] LoginModelDTO model)
    {
        await userService.ResetPassword(model.Username, model.Password);
        return Ok();
    }
}