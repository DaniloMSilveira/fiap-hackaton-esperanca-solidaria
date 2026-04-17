using EsperancaSolidaria.BuildingBlocks.Domain;

namespace EsperancaSolidaria.Domain.Entities;

public class Entity : BaseEntity
{
    public Guid Id { get; private set; }

    public Entity() => Id = Guid.NewGuid();

    public Entity(Guid id) => Id = id;

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right) => !(left == right);

    #endregion
}