using Godot;
using System;

public partial class LavaGeyser : Area2D
{
	[Export] public double InitialDelay = 0.0;
	[Export] public double DormantTime = 4.0; // Temps en "idle"
	[Export] public double WarningTime = 2.0; // Temps en "warning"
	[Export] public double ActiveTime = 1.5;  // Temps en "active" (éruption)

	private Timer _stateTimer;
	private CollisionShape2D _collision;
	private AnimatedSprite2D _geyserAnimation;

	private int _state = 0; // 0 = Idle, 1 = Warning, 2 = Active

	public override void _Ready()
	{
		_collision = GetNode<CollisionShape2D>("CollisionShape2D");
		_geyserAnimation = GetNode<AnimatedSprite2D>("GeyserAnimation");
		_stateTimer = GetNode<Timer>("StateTimer");

		_stateTimer.Timeout += OnStateTimerTimeout;

		if (InitialDelay > 0.0)
		{
			_state = 2; 
			_stateTimer.Start(InitialDelay);
		}
		else
		{
			SetStateIdle();
		}
	}

	private void OnStateTimerTimeout()
	{
		// Alterne les états : 0 -> 1 -> 2 -> 0 -> 1...
		_state = (_state + 1) % 3;

		if (_state == 0) SetStateIdle();
		else if (_state == 1) SetStateWarning();
		else if (_state == 2) SetStateActive();
	}

	private void SetStateIdle()
	{
		_collision.SetDeferred("disabled", true); // Aucun danger
		
		// Joue l'animation où il n'y a rien (ou des petites bulles calmes)
		_geyserAnimation.Play("Idle"); 

		_stateTimer.Start(DormantTime);
	}

	private void SetStateWarning()
	{
		_collision.SetDeferred("disabled", true); // Pas encore de dégâts, les flammes commencent à monter
		
		// Joue ton animation d'avertissement (les flammes qui grandissent)
		_geyserAnimation.Play("Warning"); 

		_stateTimer.Start(WarningTime);
	}

	private void SetStateActive()
	{
		_collision.SetDeferred("disabled", false); // Devient mortel !
		
		// Joue l'explosion du geyser de lave
		_geyserAnimation.Play("Active"); 

		_stateTimer.Start(ActiveTime);
	}

	private void _on_area_entered(Area2D area)
	{
		// Si un joueur touche la zone pendant l'état actif (2), il prend un dégât
		if (_state == 2 && area.Name == "FeetArea" && area.GetParent() is SOFightingCharacter joueur)
		{
			GD.Print($"Le joueur {joueur.PlayerController} a touché le geyser actif !");
			joueur.TakeDamage(1, this);
		}
	}
}
