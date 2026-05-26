using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class JudgementScene : SONodeScene
{
	[Export] public Label firstPlayerLabel;
	[Export] public Label secondPlayerLabel;
	[Export] public Label thirdPlayerLabel;
	[Export] public Label fourthPlayerLabel;
	
	
	public override void _Ready()
	{
		ShowBattleLeaderboard();
		_ = EndScene();
	}

	public void ShowBattleLeaderboard()
	{
		List<int> leaderboard = PactManager.Instance.GetBattleLeaderboard();
		List<Label> labels = new List<Label>([firstPlayerLabel, secondPlayerLabel, thirdPlayerLabel, fourthPlayerLabel]);

		int numberOfPlayers = GameManager.Instance.NumberOfPlayers;
		for (int i = 0; i < numberOfPlayers; i++)
		{
			labels[leaderboard[i]].Text = $"Position {numberOfPlayers - i}";
		}
	}
	
	
	public async Task EndScene()
	{
		await ToSignal(CreateTween().TweenInterval(5.0), Tween.SignalName.Finished);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}
}
