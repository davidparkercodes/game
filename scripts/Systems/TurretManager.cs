using Godot;

public partial class TurretManager : Node
{
	public static TurretManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		GD.Print("🎯 TurretManager ready");
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Handle global clicks to deselect turrets when clicking on empty space
		if (@event is InputEventMouseButton button && button.Pressed && button.ButtonIndex == MouseButton.Left)
		{
			// If there's a selected turret and we clicked on empty space, deselect it
			if (Turret.SelectedTurret != null)
			{
				// Check if we clicked on a turret by doing a physics query
				var spaceState = GetViewport().World2D.DirectSpaceState;
				var mousePos = GetViewport().GetMousePosition();
				
				var query = new PhysicsPointQueryParameters2D();
				query.Position = mousePos;
				query.CollisionMask = 1; // Assuming turrets are on layer 1
				
				var result = spaceState.IntersectPoint(query);
				
				// If we didn't click on any turret, deselect the current one
				bool clickedOnTurret = false;
				foreach (var hit in result)
				{
					if (hit["collider"].AsGodotObject() is Turret)
					{
						clickedOnTurret = true;
						break;
					}
				}
				
				if (!clickedOnTurret)
				{
					Turret.SelectedTurret.DeselectTurret();
					GD.Print("🚫 Deselected turret - clicked on empty space");
				}
			}
		}
		
		// Handle ESC key to deselect turrets
		if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
		{
			if (Turret.SelectedTurret != null)
			{
				Turret.SelectedTurret.DeselectTurret();
				GD.Print("🚫 Deselected turret - ESC pressed");
			}
		}
	}
}
