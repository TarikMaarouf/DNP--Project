using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        if (IsUsernameTaken(request.Username))
        {
            return Conflict($"Username '{request.Username}' is already taken");
        }

        User user = new User
        {
            Username = request.Username,
            Password = request.Password
        };
        User created = await userRepo.AddAsync(user);

        UserDto dto = ToDto(created);
        return Created($"/users/{dto.Id}", dto);
    }

    // PUT /users/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        User existingUser = await userRepo.GetSingleAsync(id);

        if (IsUsernameTaken(request.Username, ignoreUserId: id))
        {
            return Conflict($"Username '{request.Username}' is already taken");
        }

        existingUser.Username = request.Username;
        existingUser.Password = request.Password;
        await userRepo.UpdateAsync(existingUser);
        return NoContent();
    }

    // GET /users/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);
        return Ok(ToDto(user));
    }

    // GET /users?usernameContains=ali
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? usernameContains)
    {
        IQueryable<User> users = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(usernameContains))
        {
            users = users.Where(u => u.Username.Contains(usernameContains, StringComparison.OrdinalIgnoreCase));
        }

        List<UserDto> dtos = users.Select(u => ToDto(u)).ToList();
        return Ok(dtos);
    }

    // DELETE /users/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    private bool IsUsernameTaken(string username, int? ignoreUserId = null)
    {
        return userRepo.GetManyAsync()
            .Any(u => u.Id != ignoreUserId && u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    private static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };
    }
}
