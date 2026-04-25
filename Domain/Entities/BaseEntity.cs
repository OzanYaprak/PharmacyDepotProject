namespace Domain.Entities;

public abstract class BaseEntity<TId> where TId : notnull
{
    protected BaseEntity() { } // EF Core için

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedDate = DateTime.UtcNow;
    }

    public TId Id { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
}
