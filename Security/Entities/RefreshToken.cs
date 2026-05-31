using Domain.Entities.Base;

namespace Security.Entities;

public class RefreshToken : BaseEntity<int>
{
    #region Constructors

    public RefreshToken()
    {
        Token = string.Empty;
        CreatedIp = string.Empty;
    }

    public RefreshToken(int userId, string token, DateTime expires, string createdIp)
    {
        UserId = userId;
        Token = token;
        Expires = expires;
        CreatedIp = createdIp;
    }

    public RefreshToken(int id,int userId, string token, DateTime expires, string createdIp) : base(id)
    {
        UserId = userId;
        Token = token;
        Expires = expires;
        CreatedIp = createdIp;
    }

    #endregion 

    #region Properties

    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public string CreatedIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedIp { get; set; }
    public string? ReplacedToken { get; set; }
    public string? RevokeReason { get; set; }

    #endregion

    #region Navigation properties

    public virtual User User { get; set; } = null!;

    #endregion
}
