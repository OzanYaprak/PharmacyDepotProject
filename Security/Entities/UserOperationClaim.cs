using Domain.Entities.Base;

namespace Security.Entities;

public class UserOperationClaim : BaseEntity<int>
{
    #region Constructors

    public UserOperationClaim(int userId, int operationClaimId)
    {
        UserId = userId;
        OperationClaimId = operationClaimId;
    }

    public UserOperationClaim(int id, int userId, int operationClaimId) : base(id)
    {
        UserId = userId;
        OperationClaimId = operationClaimId;
    }

    #endregion

    #region Properties

    public int UserId { get; set; }
    public int OperationClaimId { get; set; }

    #endregion

    #region Navigation properties

    public virtual User User { get; set; } = null!;
    public virtual OperationClaim OperationClaim { get; set; } = null!;

    #endregion
}
