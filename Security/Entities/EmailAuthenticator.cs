using Domain.Entities.Base;

namespace Security.Entities;

public class EmailAuthenticator : BaseEntity<int>
{

    #region Constructors

    public EmailAuthenticator() { }

    public EmailAuthenticator(int userId, bool isVerified)
    {
        UserId = userId;
        IsVerified = isVerified;
    }

    public EmailAuthenticator(int id, int userId, bool isVerified) : base(id)
    {
        UserId = userId;
        IsVerified = isVerified;
    }

    #endregion

    #region Properties

    public int UserId { get; set; }
    public string? ActivationKey { get; set; }
    public bool IsVerified { get; set; }

    #endregion

    #region Navigation properties

    public virtual User User { get; set; } = null!;

    #endregion
}
