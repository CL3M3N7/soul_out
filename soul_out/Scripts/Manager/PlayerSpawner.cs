using Godot;
using Godot.Collections;
using SoulOut.Scripts.Levels;

namespace SoulOut.Scripts.Manager;

[GlobalClass]
public partial class PlayerSpawner(
	PackedScene playerScene,
	Node spawnPoints,
	Node playersNode
	) : Node
{
	public PackedScene PlayerScene = playerScene;
	public Node SpawnPoints = spawnPoints;
	public Node PlayersNode = playersNode;
	
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
		
		for (int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
		{
			// Discuter de si on fait des spawn aléatoire ou pas
			//Marker2D spawnPoint = (Marker2D) (random ? spawnPoints.PickRandom() : spawnPoints[0]);
			Marker2D spawnPoint = (Marker2D)spawnPoints[0];
			spawnPoints.Remove(spawnPoint);
			
			SOCharacter character = PlayerScene.Instantiate<SOCharacter>();
			character.GlobalPosition = spawnPoint.GlobalPosition;
			character.SpawnPosition = spawnPoint.GlobalPosition;
			character.ZIndex = 1;
			character.PlayerController = i;
			
			PlayersNode.AddChild(character);
			EmitSignal(SignalName.OnSpawnPlayer,character);
		}
	}
	
	
}
