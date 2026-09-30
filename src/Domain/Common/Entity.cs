namespace DistriFresasLY.Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public int Id { get; protected set; }

    protected Entity(int id = 0)
    {
        Id = id;
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (Id == 0 || other.Id == 0) return false;

        return Id == other.Id;
    }

    public override bool Equals(object? obj) =>
        obj is Entity entity && Equals(entity);

    public override int GetHashCode() =>
        Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity? left, Entity? right) =>
        !(left == right);
}