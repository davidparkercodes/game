using Godot;
using DomainVector2 = Game.Domain.Common.Types.Vector2;
using DomainRect2 = Game.Domain.Common.Types.Rect2;

namespace Game.Infrastructure.Common.Converters;

public static class GodotGeometryConverter
{
    public static Vector2 ToGodotVector2(DomainVector2 domainVector)
    {
        return new Vector2(domainVector.X, domainVector.Y);
    }

    public static DomainVector2 FromGodotVector2(Vector2 godotVector)
    {
        return new DomainVector2(godotVector.X, godotVector.Y);
    }

    public static Rect2 ToGodotRect2(DomainRect2 domainRect)
    {
        return new Rect2(
            ToGodotVector2(domainRect.Position),
            ToGodotVector2(domainRect.Size)
        );
    }

    public static DomainRect2 FromGodotRect2(Rect2 godotRect)
    {
        return new DomainRect2(
            FromGodotVector2(godotRect.Position),
            FromGodotVector2(godotRect.Size)
        );
    }
}
