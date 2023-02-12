using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Domain.Entities;

namespace SecurityJwt.Api.Controllers.v1;

public class UsersController : BaseController
{
    public UsersController(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    // Get --> get all users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAllEntities();
        return Ok(users);
    }

    // Get --> get user by id
    [HttpGet]
    [Route("GetUser")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _unitOfWork.Users.GetEntityById(id);
        if (user is null)
            return NotFound();
         
        return Ok(user);
    }

    // Post --> Create user
    [HttpPost]
    public async Task<IActionResult> AddUser(User user)
    {
        if(!ModelState.IsValid)
            return BadRequest();

        await _unitOfWork.Users.AddEntity(user);
        await _unitOfWork.CompleteAsync();

        return Ok(user);
    }
}
