using Godot;
using System;

public partial class FireballSpawner : Node2D
{
	[Export] public PackedScene FireballScene; // Glisse ici ta scène Fireball.tscn
	[Export] public double SpawnInterval = 3.0; // Un tir toutes les 3 secondes
	[Export] public float FireAngleDegrees = 0f; // Direction du tir (0 = Droite, 90 = Bas, 180 = Gauche, 270 = Haut)
	[Export] public double InitialDelay = 0.0; // Décalage pour désynchroniser les spawners

	private Timer _spawnTimer;
	private Marker2D _spawnPoint;
	private Vector2 _launchDirection;

	public override void _Ready()
	{
		_spawnPoint = GetNode<Marker2D>("SpawnPoint");
		_spawnTimer = GetNode<Timer>("SpawnTimer");

		_spawnTimer.Timeout += OnSpawnTimerTimeout;

		// Convertit l'angle de l'inspecteur en vecteur de direction utilisable par la physique
		_launchDirection = Vector2.FromAngle(Mathf.DegToRad(FireAngleDegrees));

		// Gestion du décalage de départ
		if (InitialDelay > 0.0)
		{
			_spawnTimer.Start(InitialDelay);
		}
		else
		{
			_spawnTimer.Start(SpawnInterval);
		}
	}

	private void OnSpawnTimerTimeout()
	{
		// Si on utilisait le délai initial, on remet le timer sur son rythme de croisière
		if (_spawnTimer.WaitTime != SpawnInterval)
		{
			_spawnTimer.Start(SpawnInterval);
		}

		SpawnFireball();
	}

	private void SpawnFireball()
	{
		if (FireballScene == null)
		{
			GD.PrintErr($"[FireballSpawner] Oubli : Glisse la scène Fireball.tscn dans l'inspecteur de {Name} !");
			return;
		}

		// 1. Instanciation de la boule de feu
		Fireball fireball = FireballScene.Instantiate<Fireball>();

		// 2. Configuration de sa position et de sa trajectoire
		fireball.GlobalPosition = _spawnPoint.GlobalPosition;
		fireball.Direction = _launchDirection;

		// 3. Ajout à la scène (on l'ajoute au parent commun pour qu'elle bouge indépendamment du spawner)
		GetParent().AddChild(fireball);
	}
}
