using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class TrialSpellcastScene : TrialScene
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
			.OfType<SpellCastCharacter>()
			.OrderByDescending(c => c.SpellScore)
			.Select(c => c.PlayerController);
		
		Array<int> leaderboard = [.. leaderboardIds];
		
		EmitSignal(TrialScene.SignalName.OnEndTrial, leaderboard);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}
}
