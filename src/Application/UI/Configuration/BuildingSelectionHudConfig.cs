using System.Collections.Generic;

namespace Game.Application.UI.Configuration;

public class BuildingSelectionHudConfig
{
    public HudLayout Layout { get; init; } = new();
    public HudStyling Styling { get; init; } = new();
    public HudAudio Audio { get; init; } = new();
    public Dictionary<string, BuildingDisplayConfig> Buildings { get; init; } = new();
}

public class HudLayout
{
    public int MaxBuildings { get; init; } = 9;
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
    public int NumberFontSize { get; init; } = 12;
}

public class HudAudio
{
    public string SelectSoundPath { get; init; } = "res://assets/audio/towers/tower_select.mp3";
    public string DeselectSoundPath { get; init; } = "res://assets/audio/towers/tower_deselect.mp3";
    public bool Enabled { get; init; } = true;
}

public class BuildingDisplayConfig
{
    public int DisplayOrder { get; set; }
    public string IconPath { get; set; } = string.Empty;
    public string IconRegion { get; set; } = string.Empty;
    public string Hotkey { get; set; } = string.Empty;
}
