using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class SniperTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig(Game.Domain.Buildings.Entities.SniperTower.ConfigKey);
		base._Ready();
	}
}
