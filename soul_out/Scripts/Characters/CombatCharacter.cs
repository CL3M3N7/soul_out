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
	
	[Export] public PackedScene LavaSplashScene;
	
	public Vector2 SpawnPosition; // Rempli par le MapManager

	public bool IsStunned { get; private set; } = false;
	public bool IsDead { get; private set; } = false;
	public bool IsKnockedBack { get; private set; } = false;
	
	private Area2D AttackArea;

	public override void _Ready()
	{
		base._Ready();
		Health = MaxHealth;
		AttackArea = GetNode<Area2D>("AttackArea");
		AttackArea.SetDeferred("monitoring", false);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsDead) return;
		
		if (IsKnockedBack)
		{
			// On applique une grosse friction pour freiner le recul rapidement
			Velocity = Velocity.MoveToward(Vector2.Zero, 3000 * (float)delta);
			MoveAndSlide(); // Fait glisser le personnage avec le moteur physique
			return; // On arrête la fonction ici pour bloquer les déplacements normaux du joueur !
		}
		
		if (IsStunned) return;
		
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
			
			// TODO : Activer le mask de l'Area2D de ton épée ici 
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
	
	public void TakeDamage(int amount = 1, Node2D attacker = null)
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
			if (attacker != null)
			{
				ApplyKnockback(attacker.GlobalPosition);
			}
			
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
		Velocity = Vector2.Zero; // On force l'arrêt des mouvements résiduels
		
		Input.StartJoyVibration(PlayerController, 0.4f, 0.8f, 0.4f);

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
		await ToSignal(tween, Tween.SignalName.Finished);
		
		IsDead = false;

		// --- IMPORTANT : On applique les dégâts APRÈS l'animation ---
		TakeDamage(); 

		// S'il n'est pas mort de sa chute, il respawn
		if (Health > 0)
		{
			GlobalPosition = SpawnPosition;
			Scale = Vector2.One;
			Rotation = 0;
			IsDead = false; // Il peut rejouer
		}
	}

	private async void Die()
	{
		IsDead = true;
		GD.Print($"Le joueur {PlayerController} est éliminé !");
		
		// On émet le signal de mort pour le CombatManager
		EmitSignal(SignalName.PlayerDied, PlayerController);
		
		// FIX : On attend une toute petite frame pour laisser l'interface (HeartHUD) 
		// s'actualiser et vider le dernier cœur avant de faire disparaître le joueur.
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		// On cache le joueur proprement
		Hide();
		
		// Désactiver ses collisions pour qu'il ne bloque pas les autres joueurs sur la map en étant invisible
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
	
	private async void ApplyKnockback(Vector2 attackerPos)
	{
		IsKnockedBack = true;

		// 1. Calcul de la direction : de l'attaquant vers la victime
		Vector2 direction = (GlobalPosition - attackerPos).Normalized();
		
		// 2. On applique une forte impulsion 
		Velocity = direction * 800f; 

		// 3. Petite vibration pour celui qui se prend le coup (moteur faible)
		Input.StartJoyVibration(PlayerController, 0.6f, 0.0f, 0.15f);

		// 4. On attend 0.2 secondes (le temps de glisser en arrière)
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);

		// 5. On rend le contrôle au joueur
		IsKnockedBack = false;
	}
}
