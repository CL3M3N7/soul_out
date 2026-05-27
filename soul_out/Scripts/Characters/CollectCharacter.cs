using Godot;
using System;

public partial class CollectCharacter : SOCharacter
{
	// --- SIGNAUX POUR L'UI ET LE MANAGER ---
	[Signal] public delegate void GoldChangedEventHandler(int newGold);

	public int Gold { get; private set; } = 0;
	public bool IsStunned { get; private set; } = false;
	
	public Vector2 SpawnPosition; // Rempli par le MapManager
	
	private Area2D CollectArea;

	public override void _Ready()
	{
		base._Ready();
		CollectArea = GetNode<Area2D>("CollectArea");
		CollectArea.SetDeferred("monitoring", false);
	}

	public override void _PhysicsProcess(double delta)
	{
		if(IsStunned) return;
		base._PhysicsProcess(delta);
		Vector2 temp = GetNode<Area2D>("CollectArea").GetPosition();
		GetNode<Area2D>("CollectArea")
			.SetPosition(new(CharacterSprite.IsFlippedH() ? -Math.Abs(temp.X) : Math.Abs(temp.X), 0));
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if(IsStunned) return;
		base._UnhandledInput(@event);
		// Ajout du '$' crucial pour l'interpolation de la variable PlayerController !
		if (@event.IsActionPressed($"SOActionButton0_{PlayerController}"))
		{
			Collect();
		}
	}
	
	public void StunCharacter()
	{
		IsStunned = true;
		Velocity = Vector2.Zero;
	}
	
	public void Collect()
	{
		if (CharacterSprite != null)
		{
			CharacterSprite.Play("collect");
			CharacterSprite.AnimationFinished += WakeUpCharacter;
			StunCharacter();
			
			CollectArea.SetDeferred("monitoring", true);
			CharacterSprite.AnimationFinished += EndCollect;
		}
		else
		{
			GD.PrintErr("[CollectCharacter] No animated sprite attached!");
		}
	}
	
	public void WakeUpCharacter()
	{
		IsStunned = false;
		CharacterSprite.AnimationFinished -= WakeUpCharacter;
	}
	
	public void EndCollect()
	{
		CollectArea.SetDeferred("monitoring", false);
		CharacterSprite.AnimationFinished -= EndCollect;
	}
	
	public void CollectGold(int NewGoldAmount)
	{
		Gold += NewGoldAmount;
	}
}
