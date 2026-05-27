using Godot;
using System;
using SoulOut.Scripts.Manager;

public partial class ScoreHUD : HBoxContainer
{
	[Export] public PackedScene ScoreUiScene { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int i = 0; i < 4; i++)
		{
			ScoreUI playerScore = ScoreUiScene.Instantiate<ScoreUI>();
			playerScore.SetAvatarAndScore(i, 0);
			AddChild(playerScore);
		}
		GD.Print("Sort de la boucle");
		Wait();
	}
	
	private async void Wait()
	{
		GD.Print("début");
		await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);
		GD.Print("fin");
		if(SceneManager.Instance != null)
		{
			SceneManager.Instance.ChangeScene();
		}
	}
}
