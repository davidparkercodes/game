using Godot;
using Game.Presentation.Inventory;

public partial class LootablePickup : Area2D
{
	[Export] public string ItemName = "Item";
	[Export] public string DialogText = "You picked something up!";

	private bool _playerInArea = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Player") _playerInArea = true;
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Player") _playerInArea = false;
	}

	public override void _Process(double delta)
	{
		if (!_playerInArea || !Input.IsActionJustPressed("interact"))
			return;

		Inventory.AddItem(ItemName);
		
		var label = GetNode<Label>("../UI/DialogBox/DialogLabel");
		var panel = GetNode<Panel>("../UI/DialogBox");

		label.Text = DialogText;
		panel.Visible = true;

		GetTree().CreateTimer(2.5f).Timeout += () => panel.Visible = false;
	}
}
