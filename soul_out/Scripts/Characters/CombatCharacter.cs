using Godot;
using System;

public partial class CombatCharacter : SOCharacter
{
	public int Health {get ; private set;}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<CharacterSprite>("Sprite");
		Health = 3;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("SOActionButton0_{PlayerController}"))
		{
			Attack();
		}
	}
	
	public void Attack()
	{
		//put attack logic
		if (_animatedSprite2D != null)
		{
			_animatedSprite2D.Play("attack");
		}
		else
		{
			GD.PrintErr("[Character] No animated sprite attached!");
		}
	}
	
	public void TakeDamage()
	{
		Health -= 1;
		if(Health <= 0)
		{
			//Maybe put a death event ?
			Die();
		}
	}
	
	private void Die()
	{
		//Death logic
	}
}
