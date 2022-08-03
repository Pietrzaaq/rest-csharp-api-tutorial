using BuberDinner.Contracts.Authentication;
using BuberDinner.Application.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using BuberDinner.Domain.Common.Errors;
using FluentResults;
using ErrorOr;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        ErrorOr<AuthenticationResult> registerResult = _authenticationService.Register(
            request.FirstName, 
            request.LastName,
            request.Email,
            request.Password);

        return registerResult.Match(
            registerResult => Ok(MapAuthResult(registerResult)),
            errors => Problem(errors));
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var authResult = _authenticationService.Login(
            request.Email,
            request.Password);

        if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: authResult.FirstError.Description
            );
        }

        return authResult.Match(
            authResult => Ok(MapAuthResult(authResult)),
            errors => Problem(errors));
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

}