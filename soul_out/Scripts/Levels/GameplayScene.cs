using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public abstract partial class GameplayScene : SONodeScene
{
	[Signal] public delegate void OnTimeOutEventHandler();
	
	
	[ExportCategory("SpawnPlayers")]
	[Export] public PackedScene PlayerScene;
	[Export] public Node SpawnPoints;
	[Export] public Node PlayersNode;
	
	[ExportCategory("Timer")]
	[Export] public int DurationInSeconds = 30;
	[Export] public Label TimerLabel;
	
	[ExportCategory("HUD")]
	[Export] public PackedScene HUDScene;
	[Export] public Control HUDContainer;

	
	public PlayerSpawner PlayerSpawner;
	public GameManager GameManager;

	private Timer _timer;
	
	private int _remainingTimeInSeconds;
	protected bool PhaseEnded = false;

	public override void _Ready()
	{
		PlayerSpawner = new PlayerSpawner(PlayerScene,SpawnPoints,PlayersNode);
		StartTimer();
		Callable.From(PlayerSpawner.SpawnPlayers).CallDeferred();
	}

	public void StartTimer()
	{
		_timer = new Timer();
		_timer.WaitTime = 1f;
		_remainingTimeInSeconds = DurationInSeconds;
		_timer.Timeout += OnTimeOutTimerSeconds;
		
		UpdateTimerLabel();
		AddChild(_timer);
		_timer.Start(); 
	}
	
	public void OnTimeOutTimerSeconds()
	{
		if (PhaseEnded)
			return;
		
		_remainingTimeInSeconds--;
		UpdateTimerLabel();
		if (_remainingTimeInSeconds <= 0)
		{
			_timer.Timeout -= OnTimeOutTimerSeconds;
			EmitSignal(SignalName.OnTimeOut);
		}
		else
		{
			_timer.Start();
		}
	}

	protected void UpdateTimerLabel()
	{
		TimerLabel.Text = $"{_remainingTimeInSeconds}";
	}
	
	public void PostEndScene(Array<int> leaderboard)
	{
		if (PhaseEnded)
			return;
		
		PhaseEnded = true;
		_ = EndScene(leaderboard);
	}

	public async Task EndScene(Array<int> leaderboard)
	{
		await ToSignal(CreateTween().TweenInterval(2.0), Tween.SignalName.Finished);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
		GD.Print("end scene:" + leaderboard);
	}

	public abstract void SetHUD(SOCharacter character);
}
