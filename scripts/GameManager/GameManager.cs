using Godot;

public partial class GameManager : Node
{
	[Export] public PackedScene HudScene;

	public static GameManager Instance { get; private set; }

	public int Money = 50;
	public int Lives = 20;
	public int Wave  = 1;

	public Hud Hud { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		Hud = HudScene?.Instantiate<Hud>();
		if (Hud == null)
		{
			GD.PrintErr("❌ HudScene export is empty or invalid.");
			return;
		}

		AddChild(Hud);

		Callable.From(() =>
		{
			Hud.UpdateMoney(Money);
			Hud.UpdateLives(Lives);
			Hud.UpdateWave(Wave);
		}).CallDeferred();
	}
}
