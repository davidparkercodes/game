using Godot;

public partial class PunchingBag : StaticBody2D
{
	public override void _Ready()
	{
		var stats = GetNode<StatsComponent>("Damageable/StatsComponent");
		stats.Died += () => QueueFree();
	}
}
