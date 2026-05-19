using Godot;
using System;

public partial class CombatCharacter : SOCharacter
{
	// --- SIGNAUX POUR L'UI ET LE MANAGER ---
	[Signal] public delegate void HealthChangedEventHandler(int newHealth);
	[Signal] public delegate void PlayerDiedEventHandler(int playerIndex);

	// --- VARIABLES DE COMBAT ---
	public int Health { get; private set; }
	public int MaxHealth { get; private set; } = 3;
	
	public Vector2 SpawnPosition; // Rempli par le MapManager
	public bool IsDead { get; private set; } = false;

	private CharacterSprite _animatedSprite2D;

	public override void _Ready()
	{
		// On n'oublie pas d'appeler le _Ready() de SOCharacter si besoin avec base._Ready();
		_animatedSprite2D = GetNode<CharacterSprite>("Sprite");
		
		Health = MaxHealth;
	}

	public override void _Input(InputEvent @event)
	{
		if (IsDead) return; // On bloque les inputs si le joueur est mort/tombe

		// Ajout du '$' crucial pour l'interpolation de la variable PlayerController !
		if (@event.IsActionPressed($"SOActionButton0_{PlayerController}"))
		{
			Attack();
		}
	}
	
	public void Attack()
	{
		if (_animatedSprite2D != null)
		{
			_animatedSprite2D.Play("attack");
			
			// TODO : Activer le mask de l'Area2D de ton épée ici 
			// (ex: EpeeArea.SetDeferred("monitoring", true); )
		}
		else
		{
			GD.PrintErr("[CombatCharacter] No animated sprite attached!");
		}
	}
	
	public void TakeDamage(int amount = 1)
	{
		if (IsDead) return;

		Health -= amount;
		Health = Mathf.Clamp(Health, 0, MaxHealth);
		
		// On prévient l'interface (HeartHUD) de mettre à jour les cœurs !
		EmitSignal(SignalName.HealthChanged, Health);

		if (Health <= 0)
		{
			Die();
		}
		else
		{
			// Optionnel : Jouer une animation de dégâts
			// _animatedSprite2D.Play("hurt");
		}
	}
	
	// --- LOGIQUE DE LAVE (Mélangée avec les dégâts) ---
	public async void TomberDansLaLave()
	{
		if (IsDead) return;
		
		// On fige temporairement le joueur pendant sa chute
		IsDead = true; 
		
		// Animation de chute
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.5f);
		tween.Parallel().TweenProperty(this, "rotation", Mathf.Pi * 2, 0.5f);
		await ToSignal(tween, Tween.SignalName.Finished);

		// La lave fait perdre 1 PV
		TakeDamage(1); 

		// S'il n'est pas mort de sa chute, il respawn
		if (Health > 0)
		{
			GlobalPosition = SpawnPosition;
			Scale = Vector2.One;
			Rotation = 0;
			IsDead = false; // Il peut rejouer
		}
	}

	private void Die()
	{
		IsDead = true;
		GD.Print($"Le joueur {PlayerController} est éliminé !");
		
		// _animatedSprite2D.Play("death");
		
		// On prévient le GameManager que ce joueur a perdu
		EmitSignal(SignalName.PlayerDied, PlayerController);
		
		// On cache le joueur ou on le supprime (à voir selon ton gameplay)
		Hide();
		// QueueFree(); 
	}
}
