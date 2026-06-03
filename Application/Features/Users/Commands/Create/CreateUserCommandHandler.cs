using Application.Features.Users.Rules;
using AutoMapper;
using MediatR;
using Persistence.Repositories.User;
using Security.Entities;
using Security.Hashing;

namespace Application.Features.Users.Commands.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreatedUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UserBusinessRules _userBusinessRules;

    public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userBusinessRules = userBusinessRules;
    }

    public async Task<CreatedUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(request.Email);
        User user = _mapper.Map<User>(request);

        HashingHelper.CreatePasswordHash(
            request.Password,
            passwordHash: out byte[] passwordHash,
            passwordSalt: out byte[] passwordSalt
        );
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        User createdUser = await _userRepository.AddAsync(user, cancellationToken);

        CreatedUserResponse response = _mapper.Map<CreatedUserResponse>(createdUser);
        return response;
    }
}