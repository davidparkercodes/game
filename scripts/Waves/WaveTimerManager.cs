using Godot;
using System;
using System.Collections.Generic;

public partial class WaveTimerManager : Node
{
	private List<Timer> _activeTimers = new();
	private Node _parentNode;

	public WaveTimerManager(Node parentNode)
	{
		_parentNode = parentNode;
	}

	public Timer CreateTimer(float waitTime, Action onTimeout)
	{
		var timer = new Timer();
		timer.WaitTime = waitTime;
		timer.OneShot = true;
		
		_parentNode.AddChild(timer);
		_activeTimers.Add(timer);
		
		timer.Timeout += onTimeout;
		timer.Start();
		
		GD.Print($"⏱️ Created timer with {waitTime}s wait time. Active timers: {_activeTimers.Count}");
		return timer;
	}

	public Timer CreateRepeatingTimer(float waitTime, Action onTimeout)
	{
		var timer = new Timer();
		timer.WaitTime = waitTime;
		timer.OneShot = false;
		
		_parentNode.AddChild(timer);
		_activeTimers.Add(timer);
		
		timer.Timeout += onTimeout;
		timer.Start();
		
		GD.Print($"⏱️ Created repeating timer with {waitTime}s interval. Active timers: {_activeTimers.Count}");
		return timer;
	}

	public void CleanupTimers()
	{
		GD.Print($"🧹 Cleaning up {_activeTimers.Count} timers");
		foreach (var timer in _activeTimers)
		{
			if (IsInstanceValid(timer))
			{
				timer.QueueFree();
			}
		}
		_activeTimers.Clear();
		GD.Print("🧹 Timer cleanup complete");
	}

	public void RemoveTimer(Timer timer)
	{
		if (_activeTimers.Contains(timer))
		{
			_activeTimers.Remove(timer);
			if (IsInstanceValid(timer))
			{
				timer.QueueFree();
			}
		}
	}

	public int GetActiveTimerCount() => _activeTimers.Count;
}
