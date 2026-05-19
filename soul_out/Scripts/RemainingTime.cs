using Godot;
using System;

public partial class RemainingTime : Label
{
	[Export] Timer _mainTime;
	
	public void Start()
	{
		Text = Math.Round(_mainTime.TimeLeft).ToString();
		Timer _countdown = GetNode<Timer>("OneSecond");
		_countdown.Timeout += UpdateTime;
		_countdown.OneShot = false;
		_countdown.Start();
	}
	
	public void UpdateTime()
	{
		Text = Math.Round(_mainTime.TimeLeft).ToString();
	}
}
