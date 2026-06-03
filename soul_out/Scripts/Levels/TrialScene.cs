using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters.Modifiers;
using SoulOut.Scripts.Core;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class TrialScene : GameplayScene
{
    [Signal] public delegate void OnEndTrialEventHandler(Array<int> leaderboard);
    [Signal] public delegate void OnAddBuffEventHandler(SOCharacter character,TypeBuff buff);
    [Signal] public delegate void OnAddNerfEventHandler(SOCharacter character,TypeNerf nerf);

    [Export] public TypeBuff Buff = TypeBuff.None;
    [Export] public TypeNerf Nerf = TypeNerf.None;
    
    private int _idPlayerWithBuff;
    private int _idPlayerWithNerf;

    public override void _Ready()
    {
        GetPlayerWithModifiers();
        
        base._Ready();
        
        PlayerSpawner.OnSpawnPlayer += SetHUD;
        PlayerSpawner.OnSpawnPlayer += CheckModifier;
    }

    public void GetPlayerWithModifiers()
    {
        _idPlayerWithBuff = PactManager.Instance.GetPlayerWithBuff();
        _idPlayerWithNerf = PactManager.Instance.GetPlayerWithNerf();
    }
    
    public void CheckModifier(SOCharacter character)
    {
        if (character.PlayerController == _idPlayerWithBuff)
            EmitSignalOnAddBuff(character, Buff);
        
        if (character.PlayerController == _idPlayerWithNerf)
            EmitSignalOnAddNerf(character, Nerf);
    }
    
    public override void SetHUD(SOCharacter character)
    {
        throw new System.NotImplementedException();
    }
}