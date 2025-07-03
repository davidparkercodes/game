using Godot;

public partial class Damageable : Node2D
{
	[Export] public StatsComponent Stats;

	public void ApplyDamage(int amount)
	{
		Stats.ModifyHP(-amount);  // Negative = damage
		GD.Print($"Took {amount} damage! Current HP: {Stats.CurrentHP}");
	}
}
