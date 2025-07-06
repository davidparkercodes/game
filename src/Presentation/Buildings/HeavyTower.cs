using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class HeavyTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig("heavy_tower");
		base._Ready();
	}
}
