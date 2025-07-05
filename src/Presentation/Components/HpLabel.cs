using Godot;

namespace Game.Presentation.Components;

public partial class HpLabel : Label
{
	public override void _Ready()
	{
		var stats = GetParent().GetNodeOrNull<StatsComponent>("../Damageable/StatsComponent");

		if (stats == null)
		{
			GD.PrintErr("❌ StatsComponent not found!");
			Text = "?? / ??";
			return;
		}

		stats.HpChanged += OnHpChanged;
		OnHpChanged(stats.CurrentHP, stats.MaxHP);
	}

	private void OnHpChanged(int current, int max)
	{
		Text = $"{current}/{max}";
	}
}
