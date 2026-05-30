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
	[Export] public float InvincibilityDuration = 1.5f;
	
	public Vector2 SpawnPosition; // Rempli par le MapManager

	public bool IsStunned { get; private set; } = false;
	public bool IsDead { get; private set; } = false;
	public bool IsKnockedBack { get; private set; } = false;
	public bool IsInvincible { get; private set; } = false;
	
	// --- VARIABLES DE CAPACITES ---
	public bool IsShielding { get; private set; } = false;
	public bool IsDashing { get; private set; } = false;
	private bool _canDash = true; // Pour le temps de recharge
	
	private CpuParticles2D _dashParticles;
	
	[ExportGroup("Dash Settings")]
	[Export] public float DashSpeed = 1000f; // Vitesse de la ruée
	[Export] public float DashDuration = 0.2f; // Durée de la ruée
	[Export] public float DashCooldown = 1.0f; // Temps d'attente avant de pouvoir re-dash

	[ExportGroup("Shield Settings")]
	[Export] public float ShieldKnockbackForce = 350f;
	
	private Area2D AttackArea;
	private Tween _invincibilityTween; // Pour gérer le clignotement d'invincibilité
	
	// Audio 
	private AudioStreamPlayer2D _sfxAttack;
	private AudioStreamPlayer2D _sfxHurt;
	private AudioStreamPlayer2D _sfxLava;

	public override void _Ready()
	{
		base._Ready();
		Health = _maxHealth;
		AttackArea = GetNode<Area2D>("AttackArea");
		AttackArea.SetDeferred("monitoring", false);
		
		_dashParticles = GetNode<CpuParticles2D>("DashParticles");
		
		_sfxAttack = GetNode<AudioStreamPlayer2D>("SfxAttack");
		_sfxHurt = GetNode<AudioStreamPlayer2D>("SfxHurt");
		_sfxLava = GetNode<AudioStreamPlayer2D>("SfxLava");
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
		
		if (IsDashing)
		{
			GD.Print("Vitesse appliquée pendant le dash : ", Velocity);
			MoveAndSlide(); // Maintient la vélocité définie au début du dash
			return;
		}
		
		if (IsStunned) return;
		
		if (IsShielding)
		{
			Velocity = Vector2.Zero; 
			MoveAndSlide();
			return;
		}
		
		base._PhysicsProcess(delta);

		Vector2 temp = GetNode<Area2D>("AttackArea").GetPosition();
		GetNode<Area2D>("AttackArea")
			.SetPosition(new(CharacterSprite.IsFlippedH() ? -Math.Abs(temp.X) : Math.Abs(temp.X), 0));
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (IsDead || IsStunned || IsDashing || IsKnockedBack) return; // On bloque les inputs si le joueur est mort/tombe
		base._UnhandledInput(@event);
		
		// Ajout du '$' crucial pour l'interpolation de la variable PlayerController !
		if (@event.IsActionPressed($"SOActionButton0_{PlayerController}"))
		{
			Attack();
		}
		
		if (@event.IsActionPressed($"SOActionButton1_{PlayerController}") && !IsDashing)
		{
			IsShielding = true;
			if (CharacterSprite != null)
			{
				CharacterSprite.Play("defend");
			
				//_sfxDefend?.Play();
			}
			else
			{
				GD.PrintErr("[CombatCharacter] No animated sprite attached!");
			}
		}
		
		if (@event.IsActionReleased($"SOActionButton1_{PlayerController}"))
		{
			IsShielding = false;
			if (CharacterSprite != null)
			{
				CharacterSprite.Play("idle");
			}
		}
		
		// --- DASH (Action 2) ---
		if (@event.IsActionPressed($"SOActionButton3_{PlayerController}"))
		{
			Dash();
		}
	}
	
	// --- NOUVELLE FONCTION : DASH MULTIDIRECTIONNEL ---
	private async void Dash()
	{
		GD.Print("Bouton Dash pressé !");
		if (!_canDash || IsShielding || IsDashing) return;

		_canDash = false;
		IsDashing = true;
	
		// 1. On lit la direction du joystick du joueur (REMPLACE PAR TES VRAIS NOMS D'INPUT)
		// On ajoute l'interpolation $"{PlayerController}" si tes inputs sont séparés par joueur (ex: "move_left_0")
		Vector2 inputDirection = Input.GetVector(
			$"SOMoveLeft_{PlayerController}", 
			$"SOMoveRight_{PlayerController}", 
			$"SOMoveUp_{PlayerController}", 
			$"SOMoveDown_{PlayerController}"
		);
	
		Vector2 dashDirection;

		// 2. Si le joueur incline le joystick, on dash dans cette direction
		if (inputDirection != Vector2.Zero)
		{
			dashDirection = inputDirection.Normalized(); 
			GD.Print("Bouton pressé validé !");
		}
		else
		{
			// 3. Fallback : S'il lâche le joystick (neutre), il dash droit devant lui par défaut
			dashDirection = CharacterSprite.IsFlippedH() ? Vector2.Left : Vector2.Right;
		}
		
		if (_dashParticles != null)
		{
			_dashParticles.Emitting = true;
		}
	
		// Applique la grosse vitesse dans la direction choisie
		Velocity = dashDirection * DashSpeed;
	
		// Optionnel : Ajouter un petit son ou un effet visuel ici

		// 1. On attend la fin de la ruée
		await ToSignal(GetTree().CreateTimer(DashDuration), SceneTreeTimer.SignalName.Timeout);
		IsDashing = false;
		
		if (_dashParticles != null)
		{
			_dashParticles.Emitting = false;
		}

		// 2. On lance le temps de recharge (Cooldown)
		await ToSignal(GetTree().CreateTimer(DashCooldown), SceneTreeTimer.SignalName.Timeout);
		_canDash = true; // Le joueur peut dash à nouveau !
	}
	
	public void Attack()
	{
		if (CharacterSprite != null)
		{
			CharacterSprite.Play("attack");
			CharacterSprite.AnimationFinished += WakeUpCharacter;
			StunCharacter();
			
			_sfxAttack?.Play();
			
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
		if (IsDead || IsInvincible) return;

		// --- GESTION DU BOUCLIER DIRECTIONNEL ---
		if (IsShielding && attacker != null)
		{
			// 1. On calcule le vecteur de l'attaquant vers le joueur
			// Si la valeur X est positive, l'attaquant est à droite. Si elle est négative, il est à gauche.
			float directionToAttackerX = attacker.GlobalPosition.X - GlobalPosition.X;
		
			bool attackerIsOnRight = directionToAttackerX > 0;
			bool playerIsFacingRight = !CharacterSprite.IsFlippedH(); // FlippedH = vrai signifie qu'il regarde à gauche

			// 2. On vérifie si le joueur regarde dans la direction de l'attaquant
			bool isBlockingCorrectly = (attackerIsOnRight && playerIsFacingRight) || (!attackerIsOnRight && !playerIsFacingRight);

			if (isBlockingCorrectly)
			{
				// Le blocage est réussi ! Recul léger et pas de dégâts.
				ApplyKnockback(attacker.GlobalPosition, ShieldKnockbackForce);
				// Optionnel : _sfxShieldBlock?.Play();
				return; 
			}
			// Si 'isBlockingCorrectly' est faux, le code continue et le joueur prend cher !
		}
		
		Health = Mathf.Clamp(Health - amount, 0, _maxHealth);
		
		EmitSignal(SignalName.HealthChanged, Health);

		if (Health <= 0)
		{
			_sfxHurt?.Play();
			Die();
		}
		else
		{
			if (attacker != null)
			{
				_sfxHurt?.Play();
				ApplyKnockback(attacker.GlobalPosition, 800f);
			}
			
			BecomeTemporarilyInvincible(InvincibilityDuration);
			// Optionnel : Jouer une animation de dégâts
			// _animatedSprite2D.Play("hurt");
		}
	}
	
	// --- LOGIQUE DE LAVE (Mélangée avec les dégâts) ---
	public async void TomberDansLaLave()
	{
		if (IsDead) return;
		
		IsStunned = true; 
		Velocity = Vector2.Zero;
		
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
		
		_sfxLava?.Play();
		
		// Animation de chute
		Tween tween = CreateTween();
		tween.TweenProperty(this, "scale", Vector2.Zero, 0.5f);
		tween.Parallel().TweenProperty(this, "rotation", Mathf.Pi * 2, 0.5f);
		await ToSignal(tween, Tween.SignalName.Finished);
		
		IsStunned = false;
		
		IsShielding = false;
		TakeDamage(); 
		
		if (Health > 0)
		{
			Position = SpawnPosition;
			Scale = Vector2.One;
			Rotation = 0;
			IsStunned = false;
			
			BecomeTemporarilyInvincible(2.0f);
		}
	}
	
	public async void BecomeTemporarilyInvincible(float duration)
	{
		// Sécurité si déjà invincible
		if (IsInvincible && _invincibilityTween != null && _invincibilityTween.IsValid()) return;

		IsInvincible = true;

		// Création d'un Tween pour faire clignoter l'opacité du Sprite
		if (CharacterSprite != null)
		{
			_invincibilityTween = CreateTween().SetLoops(); // .SetLoops() fait tourner en boucle
			// Descend à 30% d'opacité en 0.1s
			_invincibilityTween.TweenProperty(CharacterSprite, "modulate:a", 0.3f, 0.1f);
			// Remonte à 100% d'opacité en 0.1s
			_invincibilityTween.TweenProperty(CharacterSprite, "modulate:a", 1.0f, 0.1f);
		}

		// On attend la fin du temps d'invincibilité
		await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);

		// Fin de l'invincibilité
		IsInvincible = false;

		// On nettoie le Tween et on remet l'opacité d'origine à coup sûr
		if (_invincibilityTween != null && _invincibilityTween.IsValid())
		{
			_invincibilityTween.Kill();
		}
		if (CharacterSprite != null)
		{
			CharacterSprite.Modulate = new Color(CharacterSprite.Modulate.R, CharacterSprite.Modulate.G, CharacterSprite.Modulate.B, 1.0f);
		}
	}

	private async void Die()
	{
		IsDead = true;
		GD.Print($"Le joueur {PlayerController} est éliminé !");
		
		// On émet le signal de mort pour le CombatManager
		EmitSignal(SignalName.OnPlayerDeath, PlayerController);
		
		// FIX : On attend une toute petite frame pour laisser l'interface (HeartHUD) 
		// s'actualiser et vider le dernier cœur avant de faire disparaître le joueur.
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		
		// On cache le joueur proprement
		Hide();
		
		// Désactiver ses collisions pour qu'il ne bloque pas les autres joueurs sur la map en étant invisible
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
		if (HasNode("FeetArea"))
		{
			// Si la FeetArea possède son propre CollisionShape2D enfant :
			GetNode<CollisionShape2D>("FeetArea/CollisionShape2D").SetDeferred("disabled", true);
		}
	
	}
	
	private async void ApplyKnockback(Vector2 attackerPos, float force)
	{
		IsKnockedBack = true;

		// 1. Calcul de la direction : de l'attaquant vers la victime
		Vector2 direction = (GlobalPosition - attackerPos).Normalized();
		
		// 2. On applique une forte impulsion 
		Velocity = direction * force; 

		// 3. Petite vibration pour celui qui se prend le coup (moteur faible)
		float vibrationIntensity = (force > 500f) ? 0.6f : 0.2f;
		Input.StartJoyVibration(PlayerController, vibrationIntensity, 0.0f, 0.15f);

		// 4. On attend 0.2 secondes (le temps de glisser en arrière)
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);

		// 5. On rend le contrôle au joueur
		IsKnockedBack = false;
	}
}
