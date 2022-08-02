using FluentResults;

namespace BuberDinner.Application.Services.Authentication;

public interface IAuthenticationService
{
    Result<AuthenticationResult> Register(string firstName, string LastName, string email, string password);

    AuthenticationResult Login(string email, string password);
}