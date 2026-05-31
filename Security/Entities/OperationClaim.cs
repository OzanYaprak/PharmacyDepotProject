using Domain.Entities.Base;

namespace Security.Entities;

public class OperationClaim: BaseEntity<int>
{
    #region Constructors

    public OperationClaim()
    {
        Name = string.Empty;
    }

    public OperationClaim(string name)
    {
        Name = name;
    }

    public OperationClaim(int id, string name) : base(id)
    {
        Name = name;
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    #endregion

    #region Navigation properties

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = new List<UserOperationClaim>();

    #endregion
}
