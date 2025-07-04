// scripts/Buildings/SniperTurret.cs
using Godot;

public partial class SniperTurret : Building
{
	protected override void ConfigureStats()
	{
		Cost = 35;
		Damage = 35;
		Range = 200.0f;
		FireRate = 2.5f;
	}

	protected override void PlayShootSound()
	{
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.PlaySound("sniper_turret_shoot");
		}
	}

	protected override string GetImpactSoundKey()
	{
		return "sniper_bullet_impact";
	}
}
