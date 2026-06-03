using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters.Modifiers;
using SoulOut.Scripts.Core;

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

    public int GetPlayerWithBuff()
    {
        return BattleLeaderboard.First();
    }

    public int GetPlayerWithNerf()
    {
        return BattleLeaderboard.Last();
    }
    
    public void AddBuff(SOCharacter character, TypeBuff buff)
    {
        character.ParticlesBuff.Emitting = true;
        if (buff == TypeBuff.None) return;
        Modifier buffNode = buff.CreateInstance();
        if (buffNode != null) character.AddChild(buffNode);
    }
    
    public void AddNerf(SOCharacter character, TypeNerf nerf)
    {
        character.ParticlesNerf.Emitting = true;
        if (nerf == TypeNerf.None) return;
        Modifier nerfNode = nerf.CreateInstance();
        if (nerfNode != null) character.AddChild(nerfNode);
    }

}