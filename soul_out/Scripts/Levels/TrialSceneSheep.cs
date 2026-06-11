using System.Linq;
using Godot.Collections;
using SoulOut.Scripts.Characters;

namespace SoulOut.Scripts.Levels;

public partial class TrialSceneSheep : TrialScene
{
	public override void _Ready()
	{
		base._Ready();
		OnTimeOut += EndScene; // todo: urgent, revoir logic trialscene
	}
	
	public void EndScene()
	{
		var leaderboardIds = PlayersNode
			.GetChildren()
			.OfType<SOTrialCharacter>()
			.OrderByDescending(c => c.Score)
			.Select(c => c.PlayerController);
		
		Array<int> leaderboard = [.. leaderboardIds];
		
		EmitSignal(TrialScene.SignalName.OnEndTrial, leaderboard);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}
}
