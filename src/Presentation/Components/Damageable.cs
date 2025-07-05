using Godot;

namespace Game.Presentation.Components;

public partial class Damageable : Node2D
{
	[Export] public StatsComponent Stats = null!;

	public void ApplyDamage(int amount)
	{
		Stats.ModifyHP(-amount);
		GD.Print($"Took {amount} damage! Current HP: {Stats.CurrentHP}");
	}
}
