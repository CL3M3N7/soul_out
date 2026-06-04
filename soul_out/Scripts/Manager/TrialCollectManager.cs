using System;
using System.Linq;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters;

namespace SoulOut.Scripts.Manager;

public partial class TrialCollectManager : Node
{
    [Signal] public delegate void OnEndTrialEventHandler(Array<int> leaderboard);
    
    private System.Collections.Generic.Dictionary<int,CollectCharacter> _characters = new();
    
    public void SubscribeToPlayer(SOCharacter character)
    {
        if (character is CollectCharacter collectCharacter)
        {
            _characters.Add(character.PlayerController,collectCharacter);
        }
        else
        {
            GD.Print($"[BattleManager] Character {character} is not a Fighter.");
            throw new ArgumentException($"Character {character} is not a Fighter.");
        }
    }
    
    public void SubmitEndBattle()
    {
        Array<int> Leaderboard = new Array<int>(
            _characters.Values
                .OrderBy(c => c.Gold)
                .Select(c => c.PlayerController)
                .ToArray()
            );
            
        EmitSignal(SignalName.OnEndTrial,new Array<int>(Leaderboard));
    }
}