using Godot;
using System;
using System.Linq;
using SoulOut.Scripts.Manager;

public partial class ScoreHUD : HBoxContainer
{
	[Export] public PackedScene ScoreUiScene { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int[] scores = GameManager.Instance.GetAllPlayerScore();
		int[] order = OrderScore(scores);
		for(int i = 0; i < scores.Length; i++)
		{
			ScoreUI playerScore = ScoreUiScene.Instantiate<ScoreUI>();
			playerScore.SetAvatarAndScore(order[i], scores[order[i]]);
			AddChild(playerScore);
		}
		Wait();
	}
	
	private int[] OrderScore(int[] scores)
	{
		int[] order = scores
			.Select((valeur, indice) => new { Indice = indice, Valeur = valeur })
			.OrderByDescending(x => x.Valeur)
			.Select(x => x.Indice)
			.ToArray();
		return order;
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
