using Godot;
using System.Linq;
using System;
using Godot.Collections;
using SoulOut.Scripts.Characters;
using SoulOut.Scripts.Manager;

namespace SoulOut.Scripts.Levels;

public partial class TrialMusicalChair : TrialScene
{
	[Export] public PackedScene _safeArea { get; set; }
	[Export] public Node SafeZoneNode;

	[Export] public float SpawnTime { get; set; } = 7.0f;

	[Export] public float EliminationTimer { get; set; } = 3.0f;

	[Export] public Area2D _validSpawnArea;
	private Array<CollisionShape2D> _validShapes = new();

	[Export] public Timer _spawnTimer;
	[Export] public Label _spawnLabel;

	[Export] public Timer _eliminationTimer;
	[Export] public Label _eliminationLabel;
	
	private Random _random = new Random();

	private int _currentPlayerNumber = GameManager.Instance.NumberOfPlayers;

	private Array<int> Leaderboard = new Array<int>();

	public override void _Ready()
	{
		base._Ready();

		_spawnTimer.WaitTime = SpawnTime;
		_spawnTimer.OneShot = true;
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
		_eliminationTimer.WaitTime = EliminationTimer;
		_eliminationTimer.OneShot = true;
		_eliminationTimer.Timeout += OnEliminationTimerTimeout;

		foreach (Node child in _validSpawnArea.GetChildren())
		{
			if (child is CollisionShape2D shape && !shape.Disabled)
			{
				_validShapes.Add(shape);
			}
		}
		_spawnTimer.Start();
		_spawnLabel.Visible = true;
	}

	private void SpawnSafeArea()
	{
		if (_safeArea == null)
		{
			GD.PrintErr("Erreur : La scène 'SpotScene' n'est pas assignée dans l'inspecteur !");
			return;
		}

		TrialMusicSafeZone newArea = _safeArea.Instantiate<TrialMusicSafeZone>();

		newArea.Position = GetValidPosition();
		
		SafeZoneNode.AddChild(newArea);
	}

	public Vector2 GetValidPosition()
	{
		int randidx = _random.Next(0,_validShapes.Count);
		CollisionShape2D randshape = _validShapes[randidx];
		Rect2 rect = randshape.Shape.GetRect();
		Vector2 randpos = new Vector2((float)(_random.NextDouble()*(rect.End.X-rect.Position.X)+rect.Position.X),(float)(_random.NextDouble()*(rect.End.Y-rect.Position.Y)+rect.Position.Y));
		return randshape.ToGlobal(randpos);
	}

	private void OnSpawnTimerTimeout()
	{
		_spawnLabel.Visible = false;
		for(int i = 0; i < _currentPlayerNumber; i++)
		{
			SpawnSafeArea();
		}
		_eliminationLabel.Visible = true;
		_eliminationTimer.Start();
	}

	private void OnEliminationTimerTimeout()
	{
		_eliminationLabel.Visible = false;
		foreach (Node child in PlayersNode.GetChildren())
		{
			if (child is MusicChairCharacter character)
			{
				if (!character.isSafe)
				{
					Leaderboard.Add(character.PlayerController);
					_currentPlayerNumber --;
					character.QueueFree();
				}
			}
		}
		foreach (Node child in SafeZoneNode.GetChildren())
		{
			if (child is TrialMusicSafeZone safeZone)
			{
				safeZone.QueueFree();
			}
		}

		if(_currentPlayerNumber > 1)
		{
			
			_spawnTimer.Start();
			_spawnLabel.Visible = true;
		}
		else
		{
			EmitSignal(TrialScene.SignalName.OnEndTrial, Leaderboard);
			EmitSignal(SONodeScene.SignalName.OnEndScene);
		}
	}


	
}
