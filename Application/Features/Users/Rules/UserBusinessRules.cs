using Application.Features.Users.Constants;
using Application.Rules;
using CrossCuttingConcerns.Exceptions.Types;
using Persistence.Repositories.User;
using Security.Entities;
using Security.Hashing;

namespace Application.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    #region Constructor And Fields

    private readonly IUserRepository _userRepository;
    public UserBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    #endregion

    public async Task UserEmailShouldNotExistsWhenInsert(string email)
    {
        User? user = await _userRepository.GetAsync(u => u.Email == email);

        if (user != null) throw new BusinessException(UserMessages.EmailExists);
    }

    public async Task UserEmailShouldNotExistsWhenUpdate(string email, int id)
    {
        User? user = await _userRepository.GetAsync(u => u.Email == email && u.Id != id);

        if (user != null) throw new BusinessException(UserMessages.EmailExists);
    }

    public void UserShouldBeExistsWhenSelected(User? user)
    {
        if (user == null) throw new BusinessException(UserMessages.UserNotFound);
    }

    public async Task UserIdShouldBeExistsWhenSelected(int id)
    {
        bool isExists = await _userRepository.AnyAsync(u => u.Id == id);
        if (!isExists) throw new BusinessException(UserMessages.UserNotFound);
    }

    public void UserPasswordShouldBeMatched(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(UserMessages.PasswordDontMatch);
    }
}