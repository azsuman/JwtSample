using JwtSample.Dto;
using JwtSample.Identity.Users;
using JwtSample.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace JwtSample.Controllers.User;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDetailDto>>> Get()
    {
        return Ok(await _userService.GetAsync());
    }

    [HttpGet("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailDto>> Get([FromRoute] string username)
    {
        return Ok(await _userService.GetAsync(username));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] UserDto user)
    {
        await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(Get), new { username = user.UserName }, null);
    }

    [HttpPut("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> Put([FromBody] UserDto user, string username)
    {
        if (username != user.UserName)
        {
            return BadRequest(new ErrorResult(System.Net.HttpStatusCode.BadRequest));
        }
        return Ok(await _userService.UpdateAsync(user, username));
    }

    [HttpDelete("{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string username)
    {
        return Ok(await _userService.DeleteAsync(username));
    }
}
