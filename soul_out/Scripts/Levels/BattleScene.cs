using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class BattleScene : SONodeScene
{
	[Export] public PlayerSpawner PlayerSpawner;
	[Export] public BattleManager BattleManager;
	[Export] public Timer Timer;

	[Signal] public delegate void OnEndSceneEventHandler();
	
	private bool _battleEnded = false;
	
	public override void _Ready()
	{
		PlayerSpawner.OnSpawnPlayer += BattleManager.SubscribeToPlayer;
		BattleManager.OnEndBattle += PostEndScene;
		PlayerSpawner.SpawnPlayers();
		Timer.Timeout += BattleManager.RegisterRemainingSurvivors;
		Timer.Start();
	}
	
	public void PostEndScene(Array<int> leaderboard)
	{
		if (_battleEnded)
			return;
		
		_battleEnded = true;
		Timer.Timeout -= BattleManager.RegisterRemainingSurvivors;
		_ = EndScene(leaderboard);
	}

	public async Task EndScene(Array<int> leaderboard)
	{
		await ToSignal(CreateTween().TweenInterval(2.0), Tween.SignalName.Finished);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
		GD.Print("end scene:" + leaderboard);
	}
}
