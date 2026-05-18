using Godot;
using System;

public partial class leveltest2 : Node
{
	[Export] private Timer _currentTimer;
	
	public override void _Ready()
	{
		if (_currentTimer == null)
		{
			_currentTimer = GetNode<Timer>("Timer");
		}
		_currentTimer.WaitTime = 10.0f;
		_currentTimer.OneShot = true;
		
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		
		if(gameManager != null)
		{
			Callable myCallable = Callable.From(() => gameManager.EndPhase(999));
			_currentTimer.Connect(Timer.SignalName.Timeout, myCallable);
			_currentTimer.Start();
		}
	}
}
