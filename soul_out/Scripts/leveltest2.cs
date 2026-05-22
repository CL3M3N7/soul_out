using Godot;
using SoulOut.Scripts.Manager;

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
		
		if(SceneManager.Instance != null)
		{
			_currentTimer.Timeout += SceneManager.Instance.ChangeScene;
			_currentTimer.Start();
			RemainingTime label = GetNode<RemainingTime>("RemainingTime");
			label.Start();
		}
		
	}
	
	public void OnEnteredExitArea(Node2D body)
	{
		if(SceneManager.Instance != null)
		{
			SceneManager.Instance.ChangeScene();
		}
	}
}
