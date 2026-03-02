namespace MoneyTrace.Domain.Primitives;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
    }

    public Guid Id { get; protected set; } 
    public bool IsActive { get; protected set; } = true;

    public void SoftDelete()
    {
        IsActive = false;
    }
}
