using Godot;
using System.Collections.Generic;

namespace Game.Presentation.Components;

public partial class Hitbox : Area2D
{
	[Export] public int Damage = 10;

	private HashSet<Node> _alreadyHit = new();

	public override void _Ready()
	{
		GD.Print("✅ Hitbox is ready.");
		BodyEntered += OnBodyEntered;
	}

	public void ResetHits()
	{
		_alreadyHit.Clear();
	}

	private void OnBodyEntered(Node2D body)
	{
		if (_alreadyHit.Contains(body))
			return;

		_alreadyHit.Add(body);

		GD.Print($"✔ Hitbox overlapped: {body.Name}");

		if (body.HasMethod("ApplyDamage"))
		{
			GD.Print("→ Calling ApplyDamage on body");
			body.Call("ApplyDamage", Damage);
			return;
		}

		if (body.GetParent() is Node parent && parent.HasMethod("ApplyDamage"))
		{
			GD.Print("→ Calling ApplyDamage on parent");
			parent.Call("ApplyDamage", Damage);
			return;
		}

		var damageable = body.GetNodeOrNull<Node>("Damageable");
		if (damageable != null && damageable.HasMethod("ApplyDamage"))
		{
			GD.Print("→ Calling ApplyDamage on Damageable component");
			damageable.Call("ApplyDamage", Damage);
			return;
		}

		GD.Print("❌ No ApplyDamage method found.");
	}
}
