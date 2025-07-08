using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class HeavyTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig(Game.Domain.Buildings.Entities.HeavyTower.ConfigKey);
		base._Ready();
	}
}
