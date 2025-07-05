using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class BasicTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig("basic_tower");
		base._Ready();
	}
}
