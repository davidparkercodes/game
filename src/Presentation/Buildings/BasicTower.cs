using Godot;
using Game.Infrastructure.Stats.Services;

namespace Game.Presentation.Buildings;

public partial class BasicTower : Building
{
	public override void _Ready()
	{
		LoadStatsFromConfig(Game.Domain.Buildings.Entities.BasicTower.ConfigKey);
		base._Ready();
	}
}
