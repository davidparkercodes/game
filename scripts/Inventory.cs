using Godot;
using System.Collections.Generic;

public partial class Inventory : Node
{
	private static Dictionary<string, int> _items = new();

	public static void AddItem(string itemName, int quantity = 1)
	{
		if (_items.ContainsKey(itemName))
			_items[itemName] += quantity;
		else
			_items[itemName] = quantity;
	}

	public static Dictionary<string, int> GetItems()
	{
		return _items;
	}
}
