using BuberDinner.Contracts.Authentication;
using BuberDinner.Application.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using BuberDinner.Application.Common.Errors;
using FluentResults;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        Result<AuthenticationResult> registerResult = _authenticationService.Register(
            request.FirstName, 
            request.LastName,
            request.Email,
            request.Password);

        if (registerResult.IsSuccess)
        {
            return Ok(MapAuthResult(registerResult.Value));
        }

        var firstError = registerResult.Errors[0];

        if(firstError is DuplicateEmailError)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Email already exists");
        }

        return Problem();
    }

    private static AuthenticationResponse MapAuthResult(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var authResult = _authenticationService.Login(
            request.Email,
            request.Password);

        var resonse = new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token);

        return Ok(resonse);
    }

}