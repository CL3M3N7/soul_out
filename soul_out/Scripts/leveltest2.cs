using Godot;
using System;

public partial class leveltest2 : Node
{
	[Export] private Timer _currentTimer;
	[Export] private Area2D _exitArea;
	
	public override void _Ready()
	{
		if (_exitArea == null)
		{
			_exitArea = GetNode<Area2D>("Area2D");
		}
		_exitArea.BodyEntered += OnEnteredExitArea;
		if (_currentTimer == null)
		{
			_currentTimer = GetNode<Timer>("Timer");
		}
		_currentTimer.WaitTime = 15.0f;
		_currentTimer.OneShot = true;
		
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		
		if(gameManager != null)
		{
			Callable myCallable = Callable.From(() => gameManager.EndPhase(999));
			_currentTimer.Connect(Timer.SignalName.Timeout, myCallable);
			_currentTimer.Start();
		}
	}
	
	public void OnEnteredExitArea(Node2D body)
	{
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		if(gameManager != null)
		{
			gameManager.EndPhase(999);
		}
	}
}
