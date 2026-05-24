using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Manager;

[GlobalClass]
public partial class BattleManager : Node
{
	[Signal] public delegate void OnEndBattleEventHandler(Array<int> leaderboard);

	public List<int> Leaderboard = new();
	private System.Collections.Generic.Dictionary<int,SOFightingCharacter> _characters = new();
	private int _remainingPlayer;

	public override void _Ready()
	{
		_remainingPlayer = GameManager.Instance.NumberOfPlayers;
	}

	public void SubscribeToPlayer(SOCharacter character)
	{
		if (character is SOFightingCharacter fighter)
		{
			fighter.OnPlayerDeath += RegisterDeadPlayerToLeaderboard;
			_characters.Add(character.PlayerController,fighter);
		}
		else
		{
			GD.Print($"[BattleManager] Character {character} is not a Fighter.");
			throw new ArgumentException($"Character {character} is not a Fighter.");
		}
		
	}

	private void RegisterDeadPlayerToLeaderboard(int playerSlot)
	{
		RegisterPlayerToLeaderboard(playerSlot);
		_remainingPlayer--;
		
		if (_remainingPlayer <= 1)
		{
			RegisterLastSurvivor();
		}
	}
	
	private void RegisterPlayerToLeaderboard(int playerSlot)
	{
		Leaderboard.Add(playerSlot);
		_characters.Remove(playerSlot);
	}

	private void RegisterLastSurvivor()
	{
		if (_remainingPlayer > 1)
			return;

		if (_characters.Count != 1)
		{
			GD.Print($"[BattleManager] _characters should contain 1 element instead of {_characters.Count}.");
			throw new ArgumentException($"_characters should contain 1 element instead of {_characters.Count}.");
		}

		int playerSlot = _characters.First().Key;
		RegisterPlayerToLeaderboard(playerSlot);
		
		SubmitEndBattle();
	}

	public void RegisterRemainingSurvivors()
	{
		var charactersSorted =
			_characters.Values.OrderBy(c => c.Health);

		foreach (var character in charactersSorted)
				RegisterPlayerToLeaderboard(character.PlayerController);
		
		SubmitEndBattle();
	}

	public void SubmitEndBattle()
	{
		EmitSignal(SignalName.OnEndBattle,new Array<int>(Leaderboard));
	}
}
