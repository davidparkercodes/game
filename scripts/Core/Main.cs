using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
	private Panel _inventoryPanel;
	private VBoxContainer _inventoryList;

	public override void _Ready()
	{
		_inventoryPanel = GetNode<Panel>("InventoryUI/InventoryPanel");
		_inventoryList = _inventoryPanel.GetNode<VBoxContainer>("MarginContainer/InventoryList");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("toggle_inventory"))
		{
			_inventoryPanel.Visible = !_inventoryPanel.Visible;

			if (_inventoryPanel.Visible)
			{
				UpdateInventoryDisplay();
			}
		}
	}

	private void UpdateInventoryDisplay()
	{
		foreach (Node child in _inventoryList.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var item in Inventory.GetItems())
		{
			var label = new Label();
			label.Text = $"{item.Key}: {item.Value}";
			_inventoryList.AddChild(label);
		}
	}
}
