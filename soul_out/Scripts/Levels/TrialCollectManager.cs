using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Characters;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class TrialCollectManager : TrialScene
{
	[Export] public PackedScene GoldScene { get; set; }
	[Export] public PackedScene GoldHUDScene;

	// Configuration du temps (modifiables dans l'inspecteur)
	[Export] private Timer _currentTimer;
	[Export] public float IntervalleMin { get; set; } = 0.5f;        // Minimum de secondes entre deux apparitions
	[Export] public float IntervalleMax { get; set; } = 2.0f;        // Maximum de secondes entre deux apparitions

	private Timer _spawnTimer;
	private Random _random = new Random();

	public override void _Ready()
	{
		SpawnPlayers();
		_spawnTimer = new Timer();
		_spawnTimer.OneShot = true; // On gère le côté aléatoire à chaque fin de cycle
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		AddChild(_spawnTimer);

		if (_currentTimer == null)
		{
			_currentTimer = GetNode<Timer>("Timer");
		}
		_currentTimer.WaitTime = 30.0f;
		_currentTimer.OneShot = true;
		
		if(SceneManager.Instance != null)
		{
			_currentTimer.Timeout += EndScene;
			_currentTimer.Start();
			RemainingTime label = GetNode<RemainingTime>("RemainingTime");
			label.Start();
		}
		ChooseTimeNextSpawn();
	}

	private void ChooseTimeNextSpawn()
	{
		if (_currentTimer.IsStopped()) return;

		// Calcul d'un temps aléatoire entre les bornes min et max
		float nextSpawnTime = (float)(_random.NextDouble() * (IntervalleMax - IntervalleMin) + IntervalleMin);
		_spawnTimer.WaitTime = nextSpawnTime;
		_spawnTimer.Start();
	}

	private void OnSpawnTimerTimeout()
	{
		if (_currentTimer.IsStopped()) return;
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
	
	//todo : enlever avec le nouveau systeme de spawn
	
	[Export] public PackedScene PlayerScene;
	public int _remainingPlayer;
	private void SpawnPlayers()
	{
		GD.Print("1. Vérification de la PlayerScene...");
		if (PlayerScene == null)
		{
			GD.PrintErr("ERREUR FATALE : PlayerScene est vide ! Tu as oublié de glisser la scène dans l'inspecteur.");
			return; // On arrête tout ici
		}

		GD.Print("2. Recherche du nœud 'SpawnPoints'...");
		Node spawnPointsNode = GetNodeOrNull<Node>("SpawnPoints");
		if (spawnPointsNode == null)
		{
			GD.PrintErr("ERREUR FATALE : Impossible de trouver le nœud 'SpawnPoints'. Vérifie l'orthographe exacte !");
			return;
		}
		GD.Print($"-> Succès : 'SpawnPoints' trouvé avec {spawnPointsNode.GetChildCount()} enfants.");

		GD.Print("3. Vérification du GameManager...");
		if (GameManager.Instance == null)
		{
			GD.PrintErr("ERREUR FATALE : GameManager.Instance est introuvable. L'Autoload est-il bien configuré ?");
			return;
		}
		
		_remainingPlayer = GameManager.Instance.NumberOfPlayers;
		GD.Print($"-> Succès : GameManager trouvé. Nombre de joueurs attendus : {_remainingPlayer}");

		Node hudContainer = GetNode<Node>("Interface/GamerHUDs");
		
		GD.Print("4. Lancement de la boucle d'apparition...");
		for (int i = 0; i < _remainingPlayer; i++)
		{
			GD.Print($"--- Tentative de création du Joueur {i} ---");
			
			if (i >= spawnPointsNode.GetChildCount())
			{
				GD.PrintErr($"ERREUR : Il manque des Marker2D ! Pas de point d'apparition pour le joueur {i}.");
				break; 
			}

			// On utilise "as Marker2D" pour ne pas faire crasher le jeu si c'est un autre type de nœud
			Node childNode = spawnPointsNode.GetChild(i);
			Marker2D spawnPoint = childNode as Marker2D;
			
			if (spawnPoint == null)
			{
				GD.PrintErr($"ERREUR FATALE : L'enfant numéro {i} de SpawnPoints n'est pas un Marker2D ! C'est un {childNode.GetType()}.");
				continue; // On passe au joueur suivant
			}

			GD.Print($"-> Instanciation du joueur {i}...");
			Node rawPlayer = PlayerScene.Instantiate();

			if (rawPlayer is not CollectCharacter newPlayer)
			{
				GD.PrintErr($"ERREUR FATALE : La scène instanciée n'a pas de script 'CollectCharacter' à sa racine ! Elle a un script {rawPlayer.GetType()}.");
				continue;
			}
			
			// Si on arrive ici, tout va bien !
			newPlayer.PlayerController = i; 
			newPlayer.GlobalPosition = spawnPoint.GlobalPosition;
			newPlayer.SpawnPosition = spawnPoint.GlobalPosition; 
			
			GD.Print($"-> Ajout du joueur {i} à la scène (Position: {newPlayer.GlobalPosition}).");
			AddChild(newPlayer);
			
			GD.Print($"+++ Joueur {i} créé avec succès ! +++");
			
			GoldHUD playerHUD = GoldHUDScene.Instantiate<GoldHUD>();
			hudContainer.AddChild(playerHUD);
			
			newPlayer.GoldChanged += playerHUD.OnPlayerGoldChanged;
		}
	}

	public void EndScene()
	{
		var leaderboardIds = GetChildren()
			.OfType<CollectCharacter>()
			.OrderByDescending(c => c.Gold)
			.Select(c => c.PlayerController);
		
		Array<int> leaderboard = [.. leaderboardIds];
		
		EmitSignal(SignalName.OnEndTrial, leaderboard);
		EmitSignal(SONodeScene.SignalName.OnEndScene);
	}

}
