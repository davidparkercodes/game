using Godot;

public partial class StatsComponent : Node
{
	[Export] public int MaxHP = 100;
	[Export] public int Strength = 5;
	[Export] public int Defence = 2;
	[Export] public float CritChance = 0.05f; // 5 %

	public int CurrentHP { get; private set; } = 100;

	public void ModifyHP(int amount)   // negative = damage
	{
		CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, MaxHP);
		EmitSignal(SignalName.HpChanged, CurrentHP, MaxHP);
		if (CurrentHP == 0)
			EmitSignal(SignalName.Died);
	}

	[Signal] public delegate void HpChangedEventHandler(int current, int max);
	[Signal] public delegate void DiedEventHandler();
}
