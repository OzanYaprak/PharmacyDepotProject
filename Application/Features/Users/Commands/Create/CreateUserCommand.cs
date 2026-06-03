using Application.Pipelines.Authorization;
using AutoMapper;
using MediatR;
using Persistence.Repositories.User;
using Security.Entities;
using Security.Hashing;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommand : IRequest<CreatedUserResponse>, ISecuredRequest
{
    public CreateUserCommand()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
    }
    public CreateUserCommand(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string[] Roles => new[] { Admin, Write, Add };

}
