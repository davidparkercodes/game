using System.Collections.Generic;

namespace Game.Application.UI.Configuration;

public class HudLayoutConfig
{
    public Dictionary<string, HudElementLayout> HudLayouts { get; init; } = new();
    public FontSettings FontSettings { get; init; } = new();
    public HudColors Colors { get; init; } = new();
}

public class HudElementLayout
{
    public Position Position { get; init; } = new();
    public Size Size { get; init; } = new();
    public string Anchor { get; init; } = "top_left";
    public bool AutoHide { get; init; } = false;
}

public class Position
{
    public int X { get; init; } = 0;
    public int Y { get; init; } = 0;
}

public class Size
{
    public int Width { get; init; } = 100;
    public int Height { get; init; } = 30;
}

public class FontSettings
{
    public TowerStatsFontSettings TowerStats { get; init; } = new();
    public MainHudFontSettings MainHud { get; init; } = new();
}

public class TowerStatsFontSettings
{
    public int InfoFontSize { get; init; } = 10;
    public int TitleFontSize { get; init; } = 12;
    public int DescriptionFontSize { get; init; } = 8;
}

public class MainHudFontSettings
{
    public int MoneyFontSize { get; init; } = 14;
    public int LivesFontSize { get; init; } = 14;
    public int WaveFontSize { get; init; } = 14;
    public int ButtonFontSize { get; init; } = 12;
}

public class HudColors
{
    public string TextPrimary { get; init; } = "#FFFFFF";
    public string TextSecondary { get; init; } = "#CCCCCC";
    public string AccentColor { get; init; } = "#4eadc7";
    public string BackgroundColor { get; init; } = "#1B1B1B";
    public string BorderColor { get; init; } = "#555555";
    public string SelectionColor { get; init; } = "#000000";
    public string RangeStartColor { get; init; } = "#33CC33";
    public string RangeEndColor { get; init; } = "#33CC3399";
}
