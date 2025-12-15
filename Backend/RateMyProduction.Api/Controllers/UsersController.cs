// RateMyProduction.Api/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using RateMyProduction.Core.Interfaces;

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
        => Ok(await _userService.GetAllAsync());

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _userService.GetPagedAsync(page, pageSize));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("username/{username}")]
    public async Task<ActionResult<UserDto>> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDto>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("exists/username/{username}")]
    public async Task<ActionResult<bool>> UsernameExists(string username)
        => Ok(await _userService.UsernameExistsAsync(username));

    [HttpGet("exists/email/{email}")]
    public async Task<ActionResult<bool>> EmailExists(string email)
        => Ok(await _userService.EmailExistsAsync(email));
}