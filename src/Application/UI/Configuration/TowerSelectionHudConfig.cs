using System.Collections.Generic;

namespace Game.Application.UI.Configuration;

public class TowerSelectionHudConfig
{
    public HudLayout Layout { get; init; } = new();
    public HudStyling Styling { get; init; } = new();
    public Dictionary<string, TowerDisplayConfig> Towers { get; init; } = new();
}

public class HudLayout
{
    public int MaxTowers { get; init; } = 9;
    public int SquareSize { get; init; } = 48;
    public int SpacingBetweenSquares { get; init; } = 8;
    public int BottomMargin { get; init; } = 20;
    public int BorderWidth { get; init; } = 2;
}

public class HudStyling
{
    public string DefaultBorderColor { get; init; } = "#444444";
    public string SelectedBorderColor { get; init; } = "#00FF00";
    public string HoverBorderColor { get; init; } = "#FFFF00";
    public string BackgroundColor { get; init; } = "#000000AA";
    public string NumberTextColor { get; init; } = "#FFFFFF";
    public int NumberFontSize { get; init; } = 14;
}

public class TowerDisplayConfig
{
    public int DisplayOrder { get; set; }
    public string IconPath { get; set; } = string.Empty;
    public string IconRegion { get; set; } = string.Empty;
    public string Hotkey { get; set; } = string.Empty;
}
