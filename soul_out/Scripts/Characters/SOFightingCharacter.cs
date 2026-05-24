using Godot;
using System;

public partial class SOFightingCharacter : SOCharacter
{
	// --- SIGNAUX POUR L'UI ET LE MANAGER ---
	[Signal] public delegate void HealthChangedEventHandler(int newHealth);
	[Signal] public delegate void OnPlayerDeathEventHandler(int playerSlot);

	// --- VARIABLES DE COMBAT ---	
	public int Health { get; private set; }
	private int _maxHealth = 3;
	
	[Export] public PackedScene LavaSplashScene;

	public bool IsStunned { get; private set; } = false;
	public bool IsDead { get; private set; } = false;
	
	private Area2D AttackArea;

	public override void _Ready()
	{
		base._Ready();
		Health = _maxHealth;
		AttackArea = GetNode<Area2D>("AttackArea");
		AttackArea.SetDeferred("monitoring", false);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsStunned || IsDead) return;
		base._PhysicsProcess(delta);

		Vector2 temp = GetNode<Area2D>("AttackArea").GetPosition();
		GetNode<Area2D>("AttackArea")
			.SetPosition(new(CharacterSprite.IsFlippedH() ? -Math.Abs(temp.X) : Math.Abs(temp.X), 0));
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (IsDead || IsStunned) return; // On bloque les inputs si le joueur est mort/tombe
		base._UnhandledInput(@event);
		
		// Ajout du '$' crucial pour l'interpolation de la variable PlayerController !
		if (@event.IsActionPressed($"SOActionButton0_{PlayerController}"))
		{
			Attack();
		}
	}
	
	public void Attack()
	{
		if (CharacterSprite != null)
		{
			CharacterSprite.Play("attack");
			CharacterSprite.AnimationFinished += WakeUpCharacter;
			StunCharacter();
			
			AttackArea.SetDeferred("monitoring", true);
			CharacterSprite.AnimationFinished += EndAttack;
		}
		else
		{
			GD.PrintErr("[CombatCharacter] No animated sprite attached!");
		}
	}
	
	public void EndAttack()
	{
		AttackArea.SetDeferred("monitoring", false);
		CharacterSprite.AnimationFinished -= EndAttack;
	}

	public void StunCharacter()
	{
		IsStunned = true;
		Velocity = Vector2.Zero;
	}
	
	public void WakeUpCharacter()
	{
		IsStunned = false;
		CharacterSprite.AnimationFinished -= WakeUpCharacter;
	}
	
	public void TakeDamage(int amount = 1)
	{
		if (IsDead) return;
		
		Health = Mathf.Clamp(Health - amount, 0, _maxHealth);
		
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
	
	private async void Die()
	{
		IsDead = true;
		GD.Print($"Le joueur {PlayerController} est éliminé !");
		
		EmitSignalOnPlayerDeath(PlayerController);
		
		// FIX : On attend une toute petite frame pour laisser l'interface (HeartHUD) 
		// s'actualiser et vider le dernier cœur avant de faire disparaître le joueur.
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		QueueFree();
	}
	
	// --- LOGIQUE DE LAVE (Mélangée avec les dégâts) ---
	public void TomberDansLaLave()
	{
		if (IsDead) return;
		
		IsStunned = true; 
		Velocity = Vector2.Zero;

		// Par sécurité : si le joueur tombait en attaquant, on nettoie l'événement
		CharacterSprite.AnimationFinished -= WakeUpCharacter;
		CharacterSprite.AnimationFinished -= EndAttack;

		if (LavaSplashScene != null)
		{
			LavaSplash splash = LavaSplashScene.Instantiate<LavaSplash>();
			splash.GlobalPosition = this.GlobalPosition;
			GetParent().AddChild(splash);
		}
		else
		{
			GD.PrintErr("[CombatCharacter] Oubli : Glisse LavaSplash.tscn dans l'inspecteur du joueur !");
		}
		
		// Animation de chute
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.5f);
		tween.Parallel().TweenProperty(this, "rotation", Mathf.Pi * 2, 0.5f);
		ToSignal(tween, Tween.SignalName.Finished);

		// --- IMPORTANT : On applique les dégâts APRÈS l'animation ---
		TakeDamage(); 

		// S'il n'est pas mort de sa chute, il respawn
		if (Health > 0)
		{
			//GlobalPosition = SpawnPosition;
			Scale = Vector2.One;
			Rotation = 0;
			IsStunned = false; // Il peut rejouer
		}
	}
}
