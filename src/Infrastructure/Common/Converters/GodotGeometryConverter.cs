using Godot;
using DomainVector2 = Game.Domain.Common.Types.Vector2;
using DomainRect2 = Game.Domain.Common.Types.Rect2;
using DomainVector2I = Game.Domain.Map.Interfaces.Vector2I;
using DomainRect2I = Game.Domain.Map.Interfaces.Rect2I;

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

    public static Vector2I ToGodotVector2I(DomainVector2I domainVector)
    {
        return new Vector2I(domainVector.X, domainVector.Y);
    }

    public static DomainVector2I FromGodotVector2I(Vector2I godotVector)
    {
        return new DomainVector2I(godotVector.X, godotVector.Y);
    }

    public static Rect2I ToGodotRect2I(DomainRect2I domainRect)
    {
        return new Rect2I(
            ToGodotVector2I(domainRect.Position),
            ToGodotVector2I(domainRect.Size)
        );
    }

    public static DomainRect2I FromGodotRect2I(Rect2I godotRect)
    {
        return new DomainRect2I(
            FromGodotVector2I(godotRect.Position),
            FromGodotVector2I(godotRect.Size)
        );
    }
}
