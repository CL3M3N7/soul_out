using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Manager;

[GlobalClass]
public partial class PlayerSpawner : Node
{
	[Export] public PackedScene PlayerScene;
	[Export] public Node SpawnPoints;
	[Export] public Node PlayersNode;

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
			Marker2D spawnPoint = (Marker2D)spawnPoints.PickRandom();
			spawnPoints.Remove(spawnPoint);
			
			SOCharacter character = PlayerScene.Instantiate<SOCharacter>();
			character.GlobalPosition = spawnPoint.GlobalPosition;
			character.ZIndex = 1;
			character.PlayerController = i;
			
			PlayersNode.AddChild(character);
		}
	}
	
	
}
