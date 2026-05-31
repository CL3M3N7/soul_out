using System;
using Godot;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class BattleScene : GameplayScene
{
	public BattleManager BattleManager;

	public override void _Ready()
	{
		base._Ready();
		
		BattleManager = new BattleManager(GameManager.Instance.NumberOfPlayers);
		
		PlayerSpawner.OnSpawnPlayer += BattleManager.SubscribeToPlayer;
		PlayerSpawner.OnSpawnPlayer += SetHUD;
		BattleManager.OnEndBattle += PostEndScene;
		OnTimeOut += BattleManager.RegisterRemainingSurvivors;
	}

	public override void SetHUD(SOCharacter character)
	{
		HeartHUD playerHUD = HUDScene.Instantiate<HeartHUD>();
		HUDContainer.AddChild(playerHUD);
		switch (character.PlayerController)
		{
			case 0:
				playerHUD.Modulate = Colors.Blue;
				break;
			case 1:
				playerHUD.Modulate = Colors.Red;
				break;
			case 2:
				playerHUD.Modulate = Colors.Gold;
				break;
			case 3:
				playerHUD.Modulate = Colors.Purple;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if (character is SOFightingCharacter fighter)
			fighter.HealthChanged += playerHUD.OnPlayerHealthChanged;
	}
}
