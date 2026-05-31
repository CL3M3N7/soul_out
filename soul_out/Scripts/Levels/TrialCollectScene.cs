using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class TrialCollectScene : TrialScene
{
	[Export] public PackedScene GoldScene { get; set; }

	// Configuration du temps (modifiables dans l'inspecteur)
	[Export] public float IntervalleMin { get; set; } = 0.5f;        // Minimum de secondes entre deux apparitions
	[Export] public float IntervalleMax { get; set; } = 2.0f;        // Maximum de secondes entre deux apparitions

	private Timer _spawnTimer;
	private Random _random = new Random();

	public override void _Ready()
	{
		base._Ready();
		
		PlayerSpawner.OnSpawnPlayer += SetHUD;
		
		_spawnTimer = new Timer();
		_spawnTimer.OneShot = true; // On gère le côté aléatoire à chaque fin de cycle
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		AddChild(_spawnTimer);
		
		ChooseTimeNextSpawn();
	}

	private void ChooseTimeNextSpawn()
	{
		if (PhaseEnded) return;

		// Calcul d'un temps aléatoire entre les bornes min et max
		float nextSpawnTime = (float)(_random.NextDouble() * (IntervalleMax - IntervalleMin) + IntervalleMin);
		_spawnTimer.WaitTime = nextSpawnTime;
		_spawnTimer.Start();
	}

	private void OnSpawnTimerTimeout()
	{
		if (PhaseEnded) return;
		
		SpawnGold();
		ChooseTimeNextSpawn();
	}

	private void SpawnGold()
	{
		if (GoldScene == null)
		{
			GD.PrintErr("Erreur : La scène 'SpotScene' n'est pas assignée dans l'inspecteur !");
			return;
		}

		GoldSpot newSpot = GoldScene.Instantiate<GoldSpot>();

		newSpot.Position = new Vector2((float)_random.NextDouble() * 600 - 300,(float)_random.NextDouble() * 400 - 200);

		// Ajout du spot à la scène principale
		AddChild(newSpot);
	}

	private void OnDurationTimerTimeout()
	{
		GD.Print("Le temps est écoulé ! Le spawner s'arrête.");
		_spawnTimer.Stop();
		SceneManager.Instance.ChangeScene();
	}

	public void EndScene()
	{
		var leaderboardIds = GetChildren()
			.OfType<CollectCharacter>()
			.OrderByDescending(c => c.Gold)
			.Select(c => c.PlayerController);
		
		Array<int> leaderboard = [.. leaderboardIds];
		
		EmitSignal(TrialScene.SignalName.OnEndTrial, leaderboard);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}


	public override void SetHUD(SOCharacter character)
	{
		GoldHUD playerHUD = HUDScene.Instantiate<GoldHUD>();
		HUDContainer.AddChild(playerHUD);
		
		if (character is CollectCharacter collectCharacter)
			collectCharacter.GoldChanged += playerHUD.OnPlayerGoldChanged;
	}
}
