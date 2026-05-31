using Domain.Entities.Base;

namespace Security.Entities;

public class OtpAuthenticator : BaseEntity<int>
{
    #region Constructors

    public OtpAuthenticator()
    {
        SecretKey = Array.Empty<byte>();
    }

    public OtpAuthenticator(int userId, byte[] secretKey, bool isVerified)
    {
        UserId = userId;
        SecretKey = secretKey;
        IsVerified = isVerified;
    }

    public OtpAuthenticator(int id, int userId, byte[] secretKey, bool isVerified) : base(id)
    {
        UserId = userId;
        SecretKey = secretKey;
        IsVerified = isVerified;
    }

    #endregion

    #region Properties

    public int UserId { get; set; }
    public byte[] SecretKey { get; set; }
    public bool IsVerified { get; set; }

    #endregion

    #region Navigation properties

    public virtual User User { get; set; } = null!;

    #endregion
}
