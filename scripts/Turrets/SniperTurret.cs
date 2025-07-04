using Godot;

public partial class SniperTurret : Turret
{
	protected override void ConfigureStats()
	{
		Cost     = 20;
		Damage   = 100;
		FireRate = 2.5f;
		Range    = 200f;
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
