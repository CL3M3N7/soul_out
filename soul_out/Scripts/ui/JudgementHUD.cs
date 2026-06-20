using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using SoulOut.Scripts.Manager;

public partial class JudgementHUD : HBoxContainer
{
	[Export] public PackedScene ScoreUiScene { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		List<int> scores = PactManager.Instance.GetBattleLeaderboard();
		for(int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
		{
			string pos;
			switch (i)
			{
				case 0:
				{
					pos = "1st";
					break;
				}
				case 1:
				{
					pos = "2nd";
					break;
				}
				case 2:
				{
					pos = "3rd";
					break;
				}
				case 3:
				{
					pos = "4th";
					break;
				}
				default:
				{
					pos = "Xth";
					break;
				}
					
			}
			
			ScoreUI playerScore = ScoreUiScene.Instantiate<ScoreUI>();
			playerScore.SetAvatarAndScore(scores[i], pos);
			AddChild(playerScore);
		}
		Wait();
	}
	
	private async void Wait()
	{
		await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);
		if(SceneManager.Instance != null)
		{
			SceneManager.Instance.ChangeScene();
		}
	}
}
