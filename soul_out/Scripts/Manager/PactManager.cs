using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Manager;

public partial class PactManager : Node
{
    public static PactManager Instance { get; private set; }

    private List<int> BattleLeaderboard = new();

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("[PactManager] Multiple instances of PactManager.");
            throw new ArgumentException("Multiple instances of PactManager.");
        }

        Instance = this;
    }
    
    public void RegisterBattleLeaderboard(Array<int> leaderboard)
    {
        BattleLeaderboard = leaderboard.ToList();
        GD.Print("PactManager :",BattleLeaderboard);
    }
    
    public List<int> GetBattleLeaderboard()
    {
        return BattleLeaderboard;
    }
}