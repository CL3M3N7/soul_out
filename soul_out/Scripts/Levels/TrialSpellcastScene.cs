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
	public void EndScene()
	{
		var leaderboardIds = GetChildren()
			.OfType<SpellCastCharacter>()
			.OrderByDescending(c => c.SpellScore)
			.Select(c => c.PlayerController);
		
		Array<int> leaderboard = [.. leaderboardIds];
		
		EmitSignal(TrialScene.SignalName.OnEndTrial, leaderboard);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}
}
