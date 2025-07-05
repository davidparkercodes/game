using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class SniperTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig("sniper_tower");
		base._Ready();
	}
}
