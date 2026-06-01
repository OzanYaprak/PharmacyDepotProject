using Domain.Entities.Base;
using Security.Enums;

namespace Security.Entities;

public class User : BaseEntity<int>
{
    #region Constructors

    public User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PasswordSalt = Array.Empty<byte>();
        PasswordHash = Array.Empty<byte>();
        IsActive = true;
    }

    public User(string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool isActive)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    public User(int id, string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash, bool isActive) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    #endregion

    #region Properties

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public AuthenticatorType AuthenticatorType { get; set; }

    #endregion

    #region Navigation properties

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = new List<UserOperationClaim>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<OtpAuthenticator> OtpAuthenticators { get; set; } = new List<OtpAuthenticator>();

    public virtual ICollection<EmailAuthenticator> EmailAuthenticators { get; set; } = new List<EmailAuthenticator>();

    #endregion
}
