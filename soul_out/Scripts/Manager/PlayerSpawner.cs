using Godot;
using Godot.Collections;
using SoulOut.Scripts.Levels;

namespace SoulOut.Scripts.Manager;

[GlobalClass]
public partial class PlayerSpawner : Node
{
	[Export] public PackedScene PlayerScene;
	[Export] public Node SpawnPoints;
	[Export] public Node PlayersNode;
	[Export] public PackedScene HeartHUDScene;
	
	[Signal] public delegate void OnSpawnPlayerEventHandler(SOCharacter character);

	public void SpawnPlayers()
	{
		if (PlayerScene == null)
		{
			GD.PrintErr("[PlayerSpawner] PlayerScene not set.");
			throw new System.Exception("PlayerScene not set.");
		}

		if (SpawnPoints == null)
		{
			GD.PrintErr("[PlayerSpawner] SpawnPoints not set.");
			throw new System.Exception("SpawnPoints not set.");
		}

		Array<Node> spawnPoints = SpawnPoints.GetChildren();
		
		Node hudContainer = GetParent().GetNode<Node>("Interface/GamerHUDs");

		for (int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
		{
			Marker2D spawnPoint = (Marker2D)spawnPoints.PickRandom();
			spawnPoints.Remove(spawnPoint);
			
			SOCharacter character = PlayerScene.Instantiate<SOCharacter>();
			character.GlobalPosition = spawnPoint.GlobalPosition;
			character.ZIndex = 1;
			character.PlayerController = i;
			
			if (GetParent() is BattleScene)
			{
				((SOFightingCharacter)character).SpawnPosition = spawnPoint.GlobalPosition;
			}
			
			PlayersNode.AddChild(character);
			EmitSignal(SignalName.OnSpawnPlayer,character);
			
			HeartHUD playerHUD = HeartHUDScene.Instantiate<HeartHUD>();
			
			hudContainer.AddChild(playerHUD);
			
			if (i == 0) playerHUD.Modulate = Colors.Blue;
			if (i == 1) playerHUD.Modulate = Colors.Red;
			if (i == 2) playerHUD.Modulate = Colors.Gold;
			if (i == 3) playerHUD.Modulate = Colors.Purple;

			if (GetParent() is BattleScene)
			{
				((SOFightingCharacter)character).HealthChanged += playerHUD.OnPlayerHealthChanged;
			}
		}
	}
	
	
}
