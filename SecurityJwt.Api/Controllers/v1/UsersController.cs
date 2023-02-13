﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecurityJwt.Api.RequestDTOs;
using SecurityJwt.Api.ResponseDTOs;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SecurityJwt.Api.Controllers.v1;

public class UsersController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(unitOfWork)
    {
        _userManager = userManager;
    }

    // Get --> get all users
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.GetAllEntities();

        var response = new List<ProfileResponseDto>();

        foreach (var user in users)
        {
            response.Add(new ProfileResponseDto
            {
                FirstName= user.FirstName,
                LastName= user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
            });
        }

        return Ok(response);
    }

    // Get --> get user by id
    [HttpGet]
    [Route("User")]
    public async Task<IActionResult> GetUser([Required]Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _unitOfWork.Users.GetEntityById(id);
        if (user is null)
            return NotFound();

        var response = new ProfileResponseDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email, 
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
        };
         
        return Ok(response);
    }

    // Post --> Create user
    [HttpPost]
    public async Task<IActionResult> Register([FromBody]UserRegisterRequestDto request)
    {
        if(!ModelState.IsValid)
            return BadRequest();

        if(request.Password != request.ConfirmPassword)
            return BadRequest();

        var identityUser = new IdentityUser
        {
            Email= request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
        };

        var isCreated = await _userManager.CreateAsync(identityUser, request.Password);

        if(!isCreated.Succeeded)
            return BadRequest();

        var user = new User
        {
            IdentityUserId = new Guid(identityUser.Id),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = request.Password,
            Status = true,
            DateCreated = DateTime.Now,
            DateOfBirth = DateTime.Now,
        };

        await _unitOfWork.Users.AddEntity(user);
        await _unitOfWork.CompleteAsync();

        var response = new UserRegisterResponseDto
        {
            JwtToken = "this is just a demo token"
        };

        return Ok(response);
    }
}
