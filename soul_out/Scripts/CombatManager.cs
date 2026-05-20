using Godot;
using System;

public partial class CombatManager : Node2D
{
	[Export] public PackedScene PlayerScene;
	[Export] private Timer _currentTimer;
	
	private int _remainingPlayer;

	public override void _Ready()
	{
		GD.Print("=== DÉBUT DU CHARGEMENT DE LA MAP ===");
		SpawnPlayers();
		GD.Print("=== FIN DU CHARGEMENT DE LA MAP ===");
	
		if (_currentTimer == null)
		{
			_currentTimer = GetNode<Timer>("Timer");
		}
		_currentTimer.WaitTime = 90.0f;
		_currentTimer.OneShot = true;
		
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		
		if(gameManager != null)
		{
			Callable myCallable = Callable.From(() => gameManager.EndPhase(999));
			_currentTimer.Connect(Timer.SignalName.Timeout, myCallable);
			_currentTimer.Start();
			RemainingTime label = GetNode<RemainingTime>("RemainingTime");
			label.Start();
		}
	}
	

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
			CombatCharacter newPlayer = rawPlayer as CombatCharacter;
			
			if (newPlayer == null)
			{
				GD.PrintErr($"ERREUR FATALE : La scène instanciée n'a pas de script 'CombatCharacter' à sa racine ! Elle a un script {rawPlayer.GetType()}.");
				continue;
			}
			
			// Si on arrive ici, tout va bien !
			newPlayer.PlayerController = i; 
			newPlayer.GlobalPosition = spawnPoint.GlobalPosition;
			newPlayer.SpawnPosition = spawnPoint.GlobalPosition; 
			
			newPlayer.PlayerDied += OnPlayerDeath;
			
			GD.Print($"-> Ajout du joueur {i} à la scène (Position: {newPlayer.GlobalPosition}).");
			AddChild(newPlayer);
			
			GD.Print($"+++ Joueur {i} créé avec succès ! +++");
		}
	}
	
	public void OnPlayerDeath(int tmp)
	{
		_remainingPlayer --;
		if(_remainingPlayer == 1)/*possible cas particulier si mort simultanée*/
		{
			LastPlayerRemain();
		}
	}
	
	public void LastPlayerRemain()
	{
		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		
		if(gameManager != null)
		{
			gameManager.EndPhase(999);
		}
	}
}
