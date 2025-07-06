using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class RapidTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig("rapid_tower");
		base._Ready();
	}
}
