using Godot;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts;

public partial class BattleScene : Node2D
{
	[Export] public PlayerSpawner PlayerSpawner;
	[Export] public BattleManager BattleManager;
	
	public override void _Ready()
	{
		PlayerSpawner.SpawnPlayers();
	}

	public override void _Process(double delta)
	{
	}
}
